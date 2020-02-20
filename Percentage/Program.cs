using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
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

            using var notifyIcon = new NotifyIcon {Visible = true};
            var item = new ToolStripMenuItem("Exit");
            item.Click += (_, __) => Application.Exit();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip {Items = {item}};

            var dpi = new Control().DeviceDpi;
            Update();

            var timer = new Timer {Interval = 10000};
            timer.Tick += (_, __) => Update();
            timer.Start();

            Application.Run();

            void Update()
            {
                var powerStatus = SystemInformation.PowerStatus;
                var batteryChargeStatus = powerStatus.BatteryChargeStatus;
                var isCharging =
                    batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging);
                var isCritical =
                    batteryChargeStatus.HasFlag(BatteryChargeStatus.Critical);
                var isLow = batteryChargeStatus.HasFlag(BatteryChargeStatus.Low);
                Brush brush;
                if (isCharging)
                {
                    brush = Brushes.RoyalBlue;
                }
                else if (isCritical)
                {
                    brush = Brushes.Red;
                }
                else if (isLow)
                {
                    brush = Brushes.OrangeRed;
                }
                else
                {
                    var isWin10LightTheme = 1.Equals(Registry.GetValue(
                        @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                        "SystemUsesLightTheme", null));
                    brush = isWin10LightTheme ? Brushes.Black : Brushes.White;
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
                    text = Math.Round(powerStatus.BatteryLifePercent * 100).ToString(CultureInfo.CurrentCulture);
                }

                var font = new Font(SystemFonts.DefaultFont.FontFamily, SystemFonts.DefaultFont.Size * (dpi/96f));
                int textWidth, textHeight;
                using (var bitmap = new Bitmap(1, 1))
                {
                    using var graphics = Graphics.FromImage(bitmap);
                    var size = graphics.MeasureString(text, font).ToSize();
                    textWidth = size.Width;
                    textHeight = size.Height;
                }

                var iconDimension = Math.Max(textWidth, textHeight);

                using (var bitmap = new Bitmap(iconDimension, iconDimension))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        var halfIconDimension = iconDimension / 2;
                        graphics.DrawString(text, font, brush, halfIconDimension - textWidth / 2,
                            halfIconDimension - textHeight / 2);
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