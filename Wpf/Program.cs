using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;
using Percentage.Wpf.Properties;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;

namespace Percentage.Wpf;

using static Environment;
using static Settings;

internal class Program
{
    internal const string Id = "f05f920a-c997-4817-84bd-c54d87e40625";

    [STAThread]
    public static void Main()
    {
        using (new Mutex(true, Id, out var isNewInstance))
        {
            if (!isNewInstance)
            {
                return;
            }

            if (Default.FirstRun)
            {
                Default.FirstRun = false;
                Default.Save();
            }

            var app = new Application {ShutdownMode = ShutdownMode.OnExplicitShutdown};

            app.DispatcherUnhandledException += (_, e) => ShowFeedbackMessage(e.Exception);

            AppDomain.CurrentDomain.UnhandledException += (_, e) => ShowFeedbackMessage(e.ExceptionObject);

            TaskScheduler.UnobservedTaskException += (_, e) => ShowFeedbackMessage(e.Exception);

            static void ShowFeedbackMessage(object exception)
            {
                MessageBox.Show("Battery Percentage Icon has run into an error. You can help to fix this by:\r\n" +
                                "1. press Ctrl+C on this message\r\n" +
                                "2. paste it in an email\r\n" +
                                "3. send it to soleon@live.com\r\n\r\n" +
                                (exception is Exception exp
                                    ? exp.ToString()
                                    : $"Error type: {exception.GetType().FullName}\r\n{exception}"),
                    "You Found An Error");
            }

            // Right click menu with "Exit" item to exit this application.
            using var exitMenuItem = new ToolStripMenuItem("Exit");
            using var settingsMenuItem = new ToolStripMenuItem("Settings");
            using var detailsMenuItem = new ToolStripMenuItem("Battery details");
            using var feedbackMenuItem = new ToolStripMenuItem("Send a feedback");
            using var menu = new ContextMenuStrip
                {Items = {detailsMenuItem, settingsMenuItem, feedbackMenuItem, exitMenuItem}};
            using var notifyIcon = new NotifyIcon {Visible = true, ContextMenuStrip = menu};

            void ActivateDialog<T>() where T : Window, new()
            {
                var existingWindow = app.Windows.OfType<T>().FirstOrDefault();
                if (existingWindow == null)
                {
                    new T().Show();
                }
                else
                {
                    existingWindow.Activate();
                }
            }

            exitMenuItem.Click += (_, _) => app.Shutdown();
            settingsMenuItem.Click += (_, _) => ActivateDialog<SettingsWindow>();
            detailsMenuItem.Click += (_, _) => ActivateDialog<DetailsWindow>();
            feedbackMenuItem.Click += (_, _) => Helper.SendFeedBack();

            // Setup variables used in the repetitively ran "Update" local function.
            (NotificationType Type, DateTime DateTime) lastNotification = (default, default);
            var chargingBrush = new SolidBrush(Default.ChargingColor);
            var lowBrush = new SolidBrush(Default.LowColor);
            var criticalBrush = new SolidBrush(Default.CriticalColor);
            Brush normalBrush;
            SetNormalBrush();

            // Show balloon notification when the tray icon is clicked.
            notifyIcon.MouseClick += (_, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ActivateDialog<DetailsWindow>();
                }
            };

            // Update battery status when the computer resumes or when the power status changes.
            SystemEvents.PowerModeChanged += (_, e) =>
            {
                if (e.Mode == PowerModes.Resume || e.Mode == PowerModes.StatusChange)
                {
                    Update();
                }
            };

            // Update battery status when the display settings change.
            // This will redrew the tray icon to ensure optimal icon resolution
            // under the current display settings.
            SystemEvents.DisplaySettingsChanged += (_, _) => Update();

            // Initial update.
            Update();

            // Setup timer to update the tray icon.
            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(Default.RefreshSeconds)};
            timer.Tick += (_, _) => Update();
            timer.Start();

            // Handle settings change.
            Default.PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Default.RefreshSeconds):
                        timer.Interval = TimeSpan.FromSeconds(Default.RefreshSeconds);
                        break;
                    case nameof(Default.ChargingColor):
                        chargingBrush.Color = Default.ChargingColor;
                        Update();
                        break;
                    case nameof(Default.LowColor):
                        lowBrush.Color = Default.LowColor;
                        Update();
                        break;
                    case nameof(Default.CriticalColor):
                        criticalBrush.Color = Default.CriticalColor;
                        Update();
                        break;
                    case nameof(Default.NormalColor):
                        SetNormalBrush();
                        Update();
                        break;
                    case nameof(Default.HighNotification):
                    case nameof(Default.LowNotification):
                    case nameof(Default.CriticalNotification):
                    case nameof(Default.FullNotification):
                    case nameof(Default.TrayIconFont):
                        Update();
                        break;
                }
            };

            void SetNormalBrush()
            {
                normalBrush = Default.NormalColor.A == 0
                    ? IsUsingLightTheme() ? Brushes.Black : Brushes.White
                    : new SolidBrush(Default.NormalColor);
            }

            // Run application and hold the thread.
            app.Run();

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
                    brush = normalBrush;
                    notifyIcon.BalloonTipTitle = null;
                    notifyIcon.BalloonTipText = "No battery detected";
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                }
                else if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Unknown))
                {
                    // When battery status is unknown.
                    trayIconText = "❓";
                    brush = normalBrush;
                    notifyIcon.BalloonTipTitle = null;
                    notifyIcon.BalloonTipText = "Battery status unknown";
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                }
                else if (percent == 100)
                {
                    notifyIcon.Icon?.Dispose();
                    notifyIcon.Icon = Resource.BatterFullIcon;
                    var powerLineText = powerStatus.PowerLineStatus == PowerLineStatus.Online
                        ? " and connected to power"
                        : null;
                    notifyIcon.BalloonTipTitle = notifyIcon.Text = "Fully charged" + powerLineText;
                    notifyIcon.BalloonTipText = "Your battery is fully charged" + powerLineText;
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Info;

                    if (!Default.FullNotification)
                    {
                        return;
                    }

                    notificationType = NotificationType.Full;
                    CheckAndSendNotification();

                    return;
                }
                else
                {
                    // When battery status is normal, display percentage in tray icon.
                    trayIconText = percent.ToString();
                    if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging))
                    {
                        // When the battery is charging.
                        brush = chargingBrush;
                        notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                        if (powerStatus.BatteryFullLifetime > 0)
                        {
                            notifyIcon.BalloonTipTitle = percent + "% charging";
                            notifyIcon.BalloonTipText =
                                GetReadableDuration(powerStatus.BatteryFullLifetime) + " until fully charged";
                        }
                        else
                        {
                            notifyIcon.BalloonTipTitle = null;
                            notifyIcon.BalloonTipText = percent + "% charging";
                        }

                        SetHighOrFullNotification();
                    }
                    else
                    {
                        // When battery is not charging.
                        if (percent <= Default.CriticalNotificationValue)
                        {
                            // When battery capacity is critical.
                            brush = criticalBrush;
                            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                            if (Default.CriticalNotification)
                            {
                                notificationType = NotificationType.Critical;
                            }
                        }
                        else if (percent <= Default.LowNotificationValue)
                        {
                            // When battery capacity is low.
                            brush = lowBrush;
                            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                            if (Default.LowNotification)
                            {
                                notificationType = NotificationType.Low;
                            }
                        }
                        else
                        {
                            // When battery capacity is normal.
                            brush = normalBrush;
                            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                            SetHighOrFullNotification();
                        }

                        if (powerStatus.BatteryLifeRemaining > 0)
                        {
                            notifyIcon.BalloonTipTitle = percent + "% " +
                                                         (powerStatus.PowerLineStatus == PowerLineStatus.Online
                                                             ? "connected (not charging)"
                                                             : "on battery");
                            notifyIcon.BalloonTipText =
                                GetReadableDuration(powerStatus.BatteryLifeRemaining) + " remaining";
                        }
                        else
                        {
                            notifyIcon.BalloonTipTitle = null;
                            notifyIcon.BalloonTipText = percent + "% " +
                                                        (powerStatus.PowerLineStatus == PowerLineStatus.Online
                                                            ? "connected (not charging)"
                                                            : "on battery");
                        }
                    }

                    void SetHighOrFullNotification()
                    {
                        if (percent == Default.HighNotificationValue && Default.HighNotification)
                        {
                            notificationType = NotificationType.High;
                        }
                        else if (percent == 100 && Default.FullNotification)
                        {
                            notificationType = NotificationType.Full;
                        }
                    }

                    // Local function to get readable time span from seconds.
                    static string GetReadableDuration(int seconds)
                    {
                        var hours = seconds / 3600;
                        var minutes = seconds % 3600 / 60;
                        return hours > 0
                            ? hours > 1
                                ? hours + " hours"
                                : hours + " hour"
                            : minutes > 0
                                ? minutes > 1
                                    ? minutes + " minutes"
                                    : minutes + "minute"
                                : seconds > 0
                                    ? "less than 1 minute"
                                    : null;
                    }
                }

                // Set tray icon tool tip based on the balloon notification texts.
                notifyIcon.Text = notifyIcon.BalloonTipTitle == null
                    ? notifyIcon.BalloonTipText
                    : notifyIcon.BalloonTipTitle + NewLine + notifyIcon.BalloonTipText;

                float dpi;
                int textWidth, textHeight;
                // Measure the rendered size of tray icon text under the current system DPI setting.
                using (var bitmap = new Bitmap(1, 1))
                {
                    SizeF size;
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        // Use the default menu font scaled to the current system DPI setting.

                        // Measure the rendering size of the tray icon text using this fort.
                        size = graphics.MeasureString(trayIconText, Default.TrayIconFont);
                        dpi = graphics.DpiX;
                    }

                    // Round the size to integer.
                    textWidth = (int) Math.Round(size.Width);
                    textHeight = (int) Math.Round(size.Height);
                }

                // Use the larger number of the text size as the dimension of the square tray icon.
                var iconDimension = (int) Math.Round(16 * (dpi / 96));

                // Draw the tray icon.
                using (var bitmap = new Bitmap(iconDimension, iconDimension))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        if (IsUsingLightTheme())
                        {
                            // Using anti aliasing provides the best clarity in Windows 10 light theme.
                            // The default ClearType rendering causes black edges around the text making
                            // it thick and pixelated.
                            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                        }

                        // Draw the text, with a starting position aim to centre align the text,
                        // but removing about 1 percent from top and left.
                        graphics.DrawString(trayIconText, Default.TrayIconFont, brush,
                            (iconDimension - textWidth) / 2f,
                            (iconDimension - textHeight) / 2f);

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

                CheckAndSendNotification();

                // Check and send notification.
                void CheckAndSendNotification()
                {
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

            static bool IsUsingLightTheme()
            {
                return 1.Equals(Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\",
                    false)?.GetValue("SystemUsesLightTheme", null));
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