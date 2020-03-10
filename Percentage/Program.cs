using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Percentage.Properties;

namespace Percentage
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            // This call is required to apply the "PerMonitorV2" DPI awareness mode defined in app.manifest.
            Application.EnableVisualStyles();

            // Use GDI based TextRenderer instead of GDI+ based Graphics class
            // for text rendering. The latter is pre .NET 2.0 which has performance
            // and localisation issues. 
            Application.SetCompatibleTextRenderingDefault(false);

            // Set the "using" scope of the tray icon to this method,
            // so that when "Main" method ends (i.e. the application exits)
            // the tray icon is disposed and removed from the system tray.
            using (var notifyIcon = new NotifyIcon {Visible = true})
            {
                // Right click menu with "Exit" item to exit this application.
                var exitMenuItem = new ToolStripMenuItem("Exit");
                exitMenuItem.Click += (_, __) => Application.Exit();
                var settingsMenuItem = new ToolStripMenuItem("Settings");
                settingsMenuItem.Click += (_, __) => new SettingsForm().Show();
                notifyIcon.ContextMenuStrip = new ContextMenuStrip {Items = {settingsMenuItem, exitMenuItem}};

                // Show balloon notification when the tray icon is clicked.
                notifyIcon.MouseClick += (_, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        notifyIcon.ShowBalloonTip(0);
                    }
                };

                // Setup variables used in the repetitively ran "Update" local function.
                (NotificationType Type, DateTime DateTime) lastNotification = (default, default);
                var settings = Settings.Default;
                var chargingBrush = new SolidBrush(settings.ChargingColor);
                var lowBrush = new SolidBrush(settings.LowColor);
                var criticalBrush = new SolidBrush(settings.CriticalColor);

                // Update battery status when the computer resumes or when the power status changes.
                SystemEvents.PowerModeChanged += (_, args) =>
                {
                    if (args.Mode == PowerModes.Resume || args.Mode == PowerModes.StatusChange)
                    {
                        Update();
                    }
                };

                // Enable auto start if this is the first run.
                if (settings.IsFirstRun)
                {
                    RegistryHelper.EnableAutoStart();
                    settings.IsFirstRun = false;
                    settings.Save();
                }

                // Initial update.
                Update();

                // Setup timer to update the tray icon.
                using (var timer = new Timer {Interval = settings.RefreshSeconds * 1000})
                {
                    timer.Tick += (_, __) => Update();
                    settings.PropertyChanged += (_, e) =>
                    {
                        switch (e.PropertyName)
                        {
                            case nameof(settings.RefreshSeconds):
                                timer.Interval = settings.RefreshSeconds * 1000;
                                break;
                            case nameof(settings.ChargingColor):
                                chargingBrush.Color = settings.ChargingColor;
                                break;
                            case nameof(settings.LowColor):
                                lowBrush.Color = settings.LowColor;
                                break;
                            case nameof(settings.CriticalColor):
                                criticalBrush.Color = settings.CriticalColor;
                                break;
                        }
                    };

                    timer.Start();

                    // Start the application and hold the thread.
                    Application.Run();
                }

                // Local function to update the tray icon.
                void Update()
                {
                    var powerStatus = SystemInformation.PowerStatus;
                    var batteryChargeStatus = powerStatus.BatteryChargeStatus;
                    var percent = (int) Math.Round(powerStatus.BatteryLifePercent * 100);

                    var notificationType = NotificationType.None;
                    Brush brush;
                    string trayIconText;
                    if (batteryChargeStatus.HasFlag(BatteryChargeStatus.NoSystemBattery))
                    {
                        // When no battery detected.
                        trayIconText = "❌";
                        SetBrush();
                        notifyIcon.Text = notifyIcon.BalloonTipText = "No battery detected";
                        notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                    }
                    else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Unknown))
                    {
                        // When battery status is unknown.
                        trayIconText = "❓";
                        SetBrush();
                        notifyIcon.Text = notifyIcon.BalloonTipText = "Battery status unknown";
                        notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                    }
                    else
                    {
                        // When battery status is normal, display percentage in tray icon.
                        trayIconText = percent.ToString(CultureInfo.CurrentCulture);
                        if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging))
                        {
                            // When the battery is charging.
                            brush = chargingBrush;
                            SetText(notifyIcon.BalloonTipTitle = percent + "% Charging",
                                powerStatus.BatteryFullLifetime,
                                " until fully charged");
                            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                            SetNotificationType();
                        }
                        else
                        {
                            // When battery is not charging.
                            if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Critical))
                            {
                                // When battery capacity is critical.
                                brush = criticalBrush;
                                notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                                if (settings.CriticalNotification)
                                {
                                    notificationType = NotificationType.Critical;
                                }
                            }
                            else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Low))
                            {
                                // When battery capacity is low.
                                brush = lowBrush;
                                notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                                if (settings.LowNotification)
                                {
                                    notificationType = NotificationType.Low;
                                }
                            }
                            else
                            {
                                // When battery capacity is normal.
                                SetBrush();
                                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                                SetNotificationType();
                            }

                            SetText(
                                // Check if power line is connected even when it's not charging.
                                notifyIcon.BalloonTipTitle =
                                    percent + "% " + (powerStatus.PowerLineStatus == PowerLineStatus.Online
                                        ? "Connected (not charging)"
                                        : "On battery"), powerStatus.BatteryLifeRemaining, "remaining");
                        }

                        void SetNotificationType()
                        {
                            if (percent == settings.HighNotificationValue && settings.HighNotification)
                            {
                                notificationType = NotificationType.High;
                            }
                            else if (percent == 100 && settings.FullNotification)
                            {
                                notificationType = NotificationType.Full;
                            }
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
                    }

                    // Local function to set normal brush according to the Windows theme used.
                    void SetBrush()
                    {
                        brush = RegistryHelper.IsUsingLightTheme() ? Brushes.Black : Brushes.White;
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
                            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

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

                    Debug.WriteLine(DateTime.Now);

                    // Check and send notification.
                    if (notificationType == NotificationType.None)
                    {
                        // No notification required.
                        return;
                    }

                    var utcNow = DateTime.UtcNow;
                    if (lastNotification.Type != notificationType)
                    {
                        // Notification required, and battery status has changed from last notification, notify user.
                        notifyIcon.ShowBalloonTip(0);
                    }
                    else
                    {
                        if (utcNow - lastNotification.DateTime > TimeSpan.FromMinutes(5))
                        {
                            // Notification required, but battery status is the same as last notification for more than 5 minutes,
                            // notify user.
                            notifyIcon.ShowBalloonTip(0);
                        }
                    }

                    lastNotification = (notificationType, utcNow);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private enum NotificationType : byte
        {
            None = 0,
            Critical,
            Low,
            High,
            Full
        }
    }
}