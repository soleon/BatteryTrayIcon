using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.Devices.Power;
using Windows.UI.ViewManagement;
using Microsoft.Win32;
using Percentage.Ui.NetFramework.Properties;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;

namespace Percentage.Ui.NetFramework;

using static Environment;
using static Settings;

internal class Program
{
    internal const string Id = "f05f920a-c997-4817-84bd-c54d87e40625";
    private static readonly UISettings UiSettings = new();

    /// <summary>
    ///     Performs a bit-block transfer of the color data corresponding to a
    ///     rectangle of pixels from the specified source device context into
    ///     a destination device context.
    /// </summary>
    /// <param name="hdc">Handle to the destination device context.</param>
    /// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
    /// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
    /// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
    /// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
    /// <param name="hdcSrc">Handle to the source device context.</param>
    /// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
    /// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
    /// <param name="dwRop">A raster-operation code.</param>
    /// <returns>
    ///     <c>true</c> if the operation succeeds, <c>false</c> otherwise. To get extended error information, call
    ///     <see cref="Marshal.GetLastWin32Error" />.
    /// </returns>
    [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight,
        [In] IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyIcon(IntPtr hIcon);

    /// <summary>
    ///     Retrieves the notify icon bounding rectangle.
    /// </summary>
    [DllImport("shell32.dll", SetLastError = true)]
    private static extern int Shell_NotifyIconGetRect([In] ref NotifyIconIdentifier identifier,
        [Out] out Rect iconLocation);

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

            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

            app.DispatcherUnhandledException += (_, e) => HandleException(e.Exception);

            AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException(e.ExceptionObject);

            TaskScheduler.UnobservedTaskException += (_, e) => HandleException(e.Exception);

            static void HandleException(object exception)
            {
                if (exception is OutOfMemoryException)
                {
                    MessageBox.Show("Battery Percentage Icon did not have enough memory to perform some work.\r\n" +
                                    "Please consider closing some running applications or background services to free up some memory.",
                        "Your system memory is running low",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
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
            }

            // Right click menu with "Exit" item to exit this application.
            using var exitMenuItem = new ToolStripMenuItem("Exit");
            using var settingsMenuItem = new ToolStripMenuItem("Settings");
            using var detailsMenuItem = new ToolStripMenuItem("Battery details");
            using var feedbackMenuItem = new ToolStripMenuItem("Send a feedback");
            using var menu = new ContextMenuStrip
                { Items = { detailsMenuItem, settingsMenuItem, feedbackMenuItem, exitMenuItem } };
            using var notifyIcon = new NotifyIcon { Visible = true, ContextMenuStrip = menu };

            // This empty icon is required to get the background color of the tray icon in the first "Update" method call.
            using (var bitmap = new Bitmap(1, 1))
            {
                var handle = bitmap.GetHicon();
                notifyIcon.Icon = Icon.FromHandle(handle);
                DestroyIcon(handle);
            }

            // Determines if the task bar is using a light color.
            bool IsLightTaskbar()
            {
                byte[] rgbValues;
                try
                {
                    var notifyIconType = typeof(NotifyIcon);
                    var iconId = (int)notifyIconType.GetField("id", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(notifyIcon);
                    var iconHandle = ((NativeWindow)notifyIconType
                            .GetField("window", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(notifyIcon))
                        .Handle;
                    var nid = new NotifyIconIdentifier
                    {
                        hWnd = iconHandle,
                        uID = (uint)iconId
                    };
                    nid.cbSize = (uint)Marshal.SizeOf(nid);
                    Shell_NotifyIconGetRect(ref nid, out var rect);

                    using var screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
                    using var screenPixelGraphics = Graphics.FromImage(screenPixel);
                    using (var screenGraphics = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        var screenGraphicsHandle = screenGraphics.GetHdc();
                        try
                        {
                            var screenPixelGraphicHandle = screenPixelGraphics.GetHdc();
                            try
                            {
                                BitBlt(screenPixelGraphicHandle, 0, 0, 1, 1, screenGraphicsHandle, rect.left, rect.top,
                                    (int)CopyPixelOperation.SourceCopy);
                            }
                            finally
                            {
                                screenPixelGraphics.ReleaseHdc();
                            }
                        }
                        finally
                        {
                            screenGraphics.ReleaseHdc();
                        }
                    }

                    var color = screenPixel.GetPixel(0, 0);
                    rgbValues = new[] { color.R, color.G, color.B };
                }
                catch
                {
                    // If anything goes wrong retrieving the color of the top left pixel of the tray icon,
                    // fall back to the current app color.
                    var color = UiSettings.GetColorValue(UIColorType.Background);
                    rgbValues = new[] { color.R, color.G, color.B };
                }

                // If any 2 values in the color RGB combination are greater than 128,
                // treat it as a light color.
                return rgbValues.Count(x => x > 128) > 2;
            }

            // These seemingly redundant calls are necessary for the menu.SystemColorsChanged event handling to work
            // before the menu is manually opened by the user. Without actually showing the menu forcing the menu to
            // initialise, its SystemColorsChanged event won't trigger.
            menu.Show();
            menu.Close();

            void ActivateDialog<T>(Action<T> windowCreated = null, Action<T> windowClosed = null)
                where T : Window, new()
            {
                var window = app.Windows.OfType<T>().FirstOrDefault();
                if (window == null)
                {
                    window = new T();
                    windowCreated?.Invoke(window);

                    void OnWindowClosed(object sender, EventArgs args)
                    {
                        window.Closed -= OnWindowClosed;
                        windowClosed?.Invoke(window);
                    }

                    window.Closed += OnWindowClosed;
                    window.Show();
                }
                else
                {
                    window.Activate();
                }
            }

            void OpenSettingsWindow()
            {
                ActivateDialog<SettingsWindow>();
            }

            void OpenDetailsWindow()
            {
                ActivateDialog<DetailsWindow>(
                    w => w.SettingsWindowRequested += OpenSettingsWindow,
                    w => w.SettingsWindowRequested -= OpenSettingsWindow);
            }

            exitMenuItem.Click += (_, _) => app.Shutdown();
            settingsMenuItem.Click += (_, _) => OpenSettingsWindow();
            detailsMenuItem.Click += (_, _) => OpenDetailsWindow();
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
                    OpenDetailsWindow();
                }
            };

            // Update battery status when the computer resumes or when the power status changes.
            SystemEvents.PowerModeChanged += (_, e) =>
            {
                if (e.Mode is PowerModes.Resume or PowerModes.StatusChange)
                {
                    Update();
                }
            };

            // Update battery status when the display settings change.
            // This will redrew the tray icon to ensure optimal icon resolution
            // under the current display settings.
            SystemEvents.DisplaySettingsChanged += (_, _) => Update();

            // Update tray icon colour when user preference changes settled down.
            var subject = new Subject<bool>();
            using var userPreferenceChangeFinalised = subject.Throttle(TimeSpan.FromMilliseconds(500))
                .ObserveOnDispatcher()
                .Subscribe(_ =>
                {
                    SetNormalBrush();
                    Update();
                });

            // This event can be triggered multiple times when Windows changes between dark and light theme.
            SystemEvents.UserPreferenceChanged += (_, _) => subject.OnNext(false);

            // Initial update.
            Update();

            // Setup timer to update the tray icon.
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Default.RefreshSeconds) };
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
                var foregroundUiColor = UiSettings.GetColorValue(UIColorType.Foreground);
                normalBrush = Default.NormalColor.A == 0
                    ? new SolidBrush(Color.FromArgb(foregroundUiColor.A, foregroundUiColor.R, foregroundUiColor.G,
                        foregroundUiColor.B))
                    : new SolidBrush(Default.NormalColor);
            }

            // Run application and hold the thread.
            app.Run();

            // Local function to update the tray icon.
            void Update()
            {
                var powerStatus = SystemInformation.PowerStatus;
                var batteryChargeStatus = powerStatus.BatteryChargeStatus;
                var percent = (int)Math.Round(powerStatus.BatteryLifePercent * 100);
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

                    // If the build number is less than 22000, it is Windows 10, otherwise it's Windows 11.
                    notifyIcon.Icon = OSVersion.Version.Build < 22000
                        ? IsLightTaskbar()
                            ? Resource.BatteryFullMetroLight
                            : Resource.BatteryFullMetroDark
                        : IsLightTaskbar()
                            ? Resource.BatteryFullFluentLight
                            : Resource.BatteryFullFluentDark;

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
                        var report = Battery.AggregateBattery.GetReport();
                        var chargeRateInMilliwatts = report.ChargeRateInMilliwatts;
                        if (chargeRateInMilliwatts > 0)
                        {
                            notifyIcon.BalloonTipTitle = percent + "% charging";

                            var fullChargeCapacityInMilliwattHours = report.FullChargeCapacityInMilliwattHours;
                            var remainingCapacityInMilliwattHours = report.RemainingCapacityInMilliwattHours;
                            if (fullChargeCapacityInMilliwattHours.HasValue &&
                                remainingCapacityInMilliwattHours.HasValue)
                            {
                                notifyIcon.BalloonTipText = Helper.GetReadableTimeSpan(TimeSpan.FromHours(
                                    (fullChargeCapacityInMilliwattHours.Value -
                                     remainingCapacityInMilliwattHours.Value) /
                                    (double)chargeRateInMilliwatts.Value)) + " until fully charged";
                            }
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
                                Helper.GetReadableTimeSpan(TimeSpan.FromSeconds(powerStatus.BatteryLifeRemaining)) +
                                " remaining";
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
                    textWidth = (int)Math.Round(size.Width);
                    textHeight = (int)Math.Round(size.Height);
                }

                // Use the larger number of the text size as the dimension of the square tray icon.
                var iconDimension = (int)Math.Round(16 * (dpi / 96));

                // Draw the tray icon.
                using (var bitmap = new Bitmap(iconDimension, iconDimension))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        if (IsLightTaskbar())
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
        }
    }

    private enum NotificationType : byte
    {
        None = 0,
        Critical,
        Low,
        High,
        Full
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NotifyIconIdentifier
    {
        public uint cbSize;
        public IntPtr hWnd;
        public uint uID;
        public readonly Guid guidItem;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public readonly int left;
        public readonly int top;
        public readonly int right;
        public readonly int bottom;
    }
}