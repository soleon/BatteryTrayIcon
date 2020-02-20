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
            using var notifyIcon = new NotifyIcon {Visible = true};
            var item = new ToolStripMenuItem("Exit");
            item.Click += (_, __) => Application.Exit();
            notifyIcon.ContextMenuStrip = new ContextMenuStrip {Items = {item}};

            Update();

            var timer = new Timer {Interval = 10000};
            timer.Tick += (_, __) => Update();
            timer.Start();

            Application.Run();

            void Update()
            {
                var text = (SystemInformation.PowerStatus.BatteryLifePercent * 100)
                    .ToString(CultureInfo.CurrentCulture);
                SizeF textSize;
                using (var bitmap = new Bitmap(1, 1))
                {
                    using var graphics = Graphics.FromImage(bitmap);
                    textSize = graphics.MeasureString(text, SystemFonts.DefaultFont);
                }

                using (var bitmap = new Bitmap((int) textSize.Width, (int) textSize.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawString(text, SystemFonts.DefaultFont,
                            1.Equals(Registry.GetValue(
                                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                                "SystemUsesLightTheme", null))
                                ? Brushes.Black
                                : Brushes.White, 0, 0);
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