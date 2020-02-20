using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Percentage
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();

            using var notifyIcon = new NotifyIcon {Visible = true, BalloonTipIcon = ToolTipIcon.Info};
            var item = new ToolStripMenuItem("Exit");
            item.Click += (_, __) => Application.Exit();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip {Items = {item}};
            Update();
            notifyIcon.Click += (_, __) => notifyIcon.ShowBalloonTip(0);

            var timer = new Timer {Interval = 10000};
            timer.Tick += (_, __) => Update();
            timer.Start();

            Application.Run();

            void Update()
            {
                var powerStatus = SystemInformation.PowerStatus;
                var batteryChargeStatus = powerStatus.BatteryChargeStatus;
                Brush brush;
                var percent = (int) Math.Round(powerStatus.BatteryLifePercent * 100);
                if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging))
                {
                    SetBrush();
                    SetText(notifyIcon.BalloonTipTitle = "Charging", powerStatus.BatteryFullLifetime, " until fully charged");
                }
                else
                {
                    if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Critical))
                    {
                        brush = Brushes.Red;
                    }
                    else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Low))
                    {
                        brush = Brushes.OrangeRed;
                    }
                    else
                    {
                        SetBrush();
                    }

                    SetText(notifyIcon.BalloonTipTitle = "On battery", powerStatus.BatteryLifeRemaining, " remaining");
                }

                void SetText(string title, int seconds, string suffix)
                {
                    if (percent == 100)
                    {
                        notifyIcon.Text = title + Environment.NewLine + (notifyIcon.BalloonTipText = "Fully charged");
                    }
                    else
                    {
                        var builder = new StringBuilder();
                        var duration = TimeSpan.FromSeconds(seconds);
                        Append(duration.Hours, "hour");
                        Append(duration.Minutes, "minute");
                        builder.Append(suffix);
                        notifyIcon.Text = title + Environment.NewLine + (notifyIcon.BalloonTipText = builder.ToString());

                        void Append(int value, string unit)
                        {
                            if (value == 0)
                            {
                                return;
                            }

                            if (builder.Length > 0)
                            {
                                builder.Append(" ");
                            }

                            builder.Append(value + " " + unit);
                            if (value > 1)
                            {
                                builder.Append("s");
                            }
                        }
                    }
                }

                void SetBrush()
                {
                    brush = 1.Equals(Registry.GetValue(
                        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                        "SystemUsesLightTheme", null))
                        ? Brushes.Black
                        : Brushes.White;
                }

                string text;
                if (batteryChargeStatus.HasFlag(BatteryChargeStatus.NoSystemBattery))
                {
                    text = "❌";
                }
                else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Unknown))
                {
                    text = "❓";
                }
                else
                {
                    text = percent.ToString(CultureInfo.CurrentCulture);
                }

                int textWidth, textHeight;
                Font font;
                using (var bitmap = new Bitmap(1, 1))
                {
                    using var graphics = Graphics.FromImage(bitmap);
                    font = SystemInformation.GetMenuFontForDpi((int) graphics.DpiX);
                    var size = graphics.MeasureString(text, font);
                    textWidth = (int) Math.Round(size.Width);
                    textHeight = (int) Math.Round(size.Height);
                }

                var iconDimension = Math.Max(textWidth, textHeight);

                using (var bitmap = new Bitmap(iconDimension, iconDimension))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawString(text, font, brush, (iconDimension - textWidth) / 2f,
                            (iconDimension - textHeight) / 2f);
                    }

                    var handle = bitmap.GetHicon();
                    try
                    {
                        notifyIcon.Icon?.Dispose();
                        notifyIcon.Icon = Icon.FromHandle(handle);
                    }
                    finally
                    {
                        DestroyIcon(handle);
                    }
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}