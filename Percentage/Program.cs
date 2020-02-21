using System;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Percentage
{
    internal static class Program
    {
        private static readonly SolidBrush ChargingBrush = new SolidBrush(Color.FromArgb(16, 137, 62));
        private static readonly SolidBrush LowBrush = new SolidBrush(Color.FromArgb(202, 80, 16));
        private static readonly SolidBrush CriticalBrush = new SolidBrush(Color.FromArgb(232, 17, 35));

        [STAThread]
        private static void Main()
        {
            // Allows this application to be DPI aware.
            // This is required to determine the current system DPI value
            // for scaling the tray icon text to support high DPI settings.
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            // This call is required to apply the above DPI mode.
            Application.EnableVisualStyles();

            // Use GDI based TextRenderer instead of GDI+ based Graphics class
            // for text rendering. The latter is pre .NET 2.0 which has performance
            // and localisation issues. 
            Application.SetCompatibleTextRenderingDefault(false);

            // Set the "using" scope of the tray icon to this method,
            // so that when "Main" method ends (i.e. the application exits)
            // the tray icon is disposed and removed from the system tray.
            using var notifyIcon = new NotifyIcon {Visible = true, BalloonTipIcon = ToolTipIcon.Info};

            // Right click menu with "Exit" item to exit this application.
            var item = new ToolStripMenuItem("Exit");
            item.Click += (_, __) => Application.Exit();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip {Items = {item}};

            // Show balloon notification when the try icon is clicked.
            notifyIcon.MouseClick += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    notifyIcon.ShowBalloonTip(0);
                }
            };

            // Initial update of the tray icon.
            Update();

            // Setup timer to update the try icon every 10 seconds.
            var timer = new Timer {Interval = 1000};
            timer.Tick += (_, __) => Update();
            timer.Start();

            // Start the application and hold the thread.
            Application.Run();

            // Local function to update the tray icon.
            void Update()
            {
                var powerStatus = SystemInformation.PowerStatus;
                var batteryChargeStatus = powerStatus.BatteryChargeStatus;
                var percent = (int) Math.Round(powerStatus.BatteryLifePercent * 100);

                // Determine if the Windows 10 light theme is in use.
                // False if dark theme is in use or registry key doesn't exist
                // for versions of Windows that don't support light theme.
                var isLightTheme = 1.Equals(Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "SystemUsesLightTheme", null));

                Brush brush;
                if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging))
                {
                    // When the battery is charging.
                    brush = ChargingBrush;
                    SetText(notifyIcon.BalloonTipTitle = "Charging", powerStatus.BatteryFullLifetime,
                        " until fully charged");
                }
                else
                {
                    // When battery is not charging.
                    if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Critical))
                    {
                        // When battery capacity is critical.
                        brush = CriticalBrush;
                    }
                    else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Low))
                    {
                        // When battery capacity is low.
                        brush = LowBrush;
                    }
                    else
                    {
                        // When battery capacity is normal.
                        SetBrush();
                    }

                    SetText(
                        // Check if power line is connected even when it's not charging.
                        notifyIcon.BalloonTipTitle = powerStatus.PowerLineStatus == PowerLineStatus.Online
                            ? "Connected (not charging)"
                            : "On battery", powerStatus.BatteryLifeRemaining, "remaining");
                }

                // Local function to set tool tip and balloon notification text.
                void SetText(string title, int seconds, string suffix)
                {
                    title += Environment.NewLine;
                    if (percent == 100)
                    {
                        // When battery is fully charged.
                        notifyIcon.Text = title + (notifyIcon.BalloonTipText = "Fully charged");
                    }
                    else
                    {
                        // When battery is not fully charged.
                        if (seconds > 0)
                        {
                            // When duration is valid, set readable duration as tool tip and balloon text.
                            var builder = new StringBuilder();
                            var duration = TimeSpan.FromSeconds(seconds);
                            Append(duration.Hours, "hour");
                            Append(duration.Minutes, "minute");
                            builder.Append(suffix);
                            notifyIcon.Text = title + (notifyIcon.BalloonTipText = builder.ToString());

                            void Append(int value, string unit)
                            {
                                if (value == 0)
                                {
                                    return;
                                }

                                builder.Append(value + " " + unit + (value > 1 ? "s " : " "));
                            }
                        }
                        else
                        {
                            // When duration is not valid, set percentage as tool tip and balloon text.
                            notifyIcon.Text = title + (notifyIcon.BalloonTipText = percent + "% remaining");
                        }
                    }
                }

                // Local function to set normal brush according to the Windows theme used.
                void SetBrush()
                {
                    brush = isLightTheme ? Brushes.Black : Brushes.White;
                }

                string trayIconText;
                if (batteryChargeStatus.HasFlag(BatteryChargeStatus.NoSystemBattery))
                {
                    // When no battery detected.
                    trayIconText = "❌";
                }
                else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Unknown))
                {
                    // When battery status is unknown.
                    trayIconText = "❓";
                }
                else
                {
                    // When battery status is normal, display percentage in tray icon.
                    trayIconText = percent.ToString(CultureInfo.CurrentCulture);
                }

                int textWidth, textHeight;
                Font font;

                // Measure the rendered size of tray icon text under the current system DPI setting.
                using (var bitmap = new Bitmap(1, 1))
                {
                    SizeF size;
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        // Use the default menu font scaled to the current system DPI setting.
                        font = SystemInformation.GetMenuFontForDpi((int) graphics.DpiX);

                        // Measure the rendering size of the tray icon text using this fort.
                        size = graphics.MeasureString(trayIconText, font);
                    }

                    // Round the size to integer.
                    textWidth = (int) Math.Round(size.Width);
                    textHeight = (int) Math.Round(size.Height);
                }

                // Use the larger number of the text size as the dimension of the square tray icon.
                var iconDimension = Math.Max(textWidth, textHeight);

                // Draw the tray icon.
                using (var bitmap = new Bitmap(iconDimension, iconDimension))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        if (isLightTheme)
                        {
                            // Using anti aliasing provides the best clarity in Windows 10 light theme.
                            // ClearType rendering causes black edges around the text making it thick
                            // and pixelated.
                            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                        }

                        // Scale the text to 1.3 times bigger.
                        graphics.ScaleTransform(1.3f, 1.3f);

                        // Draw the text, with a starting position aim to centre align the text,
                        // but removing about 1 percent from top and left.
                        graphics.DrawString(trayIconText, font, brush,
                            (iconDimension - textWidth) / 2f - textWidth * 0.1f,
                            (iconDimension - textHeight) / 2f - textHeight * 0.1f);

                        // The above scaling and start position alignments aim to remove the
                        // padding of the font so that the text fills the tray icon edge to edge.
                    }

                    // Set tray icon from the drawn bitmap image.
                    var handle = bitmap.GetHicon();
                    try
                    {
                        notifyIcon.Icon?.Dispose();
                        notifyIcon.Icon = Icon.FromHandle(handle);
                    }
                    finally
                    {
                        // Destroy icon hand to release it from memory as soon as it's set to the tray.
                        DestroyIcon(handle);

                        // Note, updating the tray icon in anyway after this call
                        // will cause the tray icon to become blank.
                        // This should be the very last call when updating the tray icon.
                    }
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}