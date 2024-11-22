using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Windows.Devices.Power;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using Percentage.App.Pages;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;
using Size = System.Windows.Size;
using TimeSpan = System.TimeSpan;
using static Percentage.App.Properties.Settings;

namespace Percentage.App;

public sealed partial class MainWindow
{
    private DispatcherTimer _batteryStatusRefreshTimer;

    // Setup variables used in the repetitively ran "Update" local function.
    private (NotificationType Type, DateTime DateTime) _lastNotification = (default, default);
    private string _notificationText;
    private string _notificationTitle;
    private IDisposable _refreshSubscription;

    // The subscriptions must be kept or the subscription can be garbage collected early.
    private IDisposable _userPreferenceChangedSubscription;
    private IDisposable _userSettingChangedSubscription;

    public MainWindow()
    {
        SystemThemeWatcher.Watch(this);
        InitializeComponent();
        App.SnackbarService.SetSnackbarPresenter(SnackbarPresenter);
    }

    private static SolidColorBrush GetBrushFromColourHexString(string hexString, Color fallbackColour)
    {
        object colour;
        try
        {
            colour = ColorConverter.ConvertFromString(hexString);
        }
        catch (FormatException)
        {
            colour = fallbackColour;
        }

        return new SolidColorBrush(colour == null ? fallbackColour : (Color)colour);
    }

    private static RenderTargetBitmap GetImageSource(FrameworkElement element)
    {
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        element.Arrange(new Rect(element.DesiredSize));
        var dpiScale = VisualTreeHelper.GetDpi(element);
        var renderTargetBitmap = new RenderTargetBitmap(
            (int)(element.ActualWidth * dpiScale.DpiScaleX),
            (int)(element.ActualHeight * dpiScale.DpiScaleY),
            dpiScale.PixelsPerInchX * 1.05,
            dpiScale.PixelsPerInchY * 1.05,
            PixelFormats.Default);
        renderTargetBitmap.Render(element);
        return renderTargetBitmap;
    }

    private SolidColorBrush GetNormalBrush()
    {
        return GetBrushFromColourHexString(Default.BatteryNormalColour,
            (Color)FindResource(nameof(ThemeResource.TextFillColorPrimary)));
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Visibility = Visibility.Collapsed;
    }

    private void OnDetailsMenuItemClick(object sender, RoutedEventArgs e)
    {
        NavigationView.Navigate(typeof(DetailsPage));
        Visibility = Visibility.Visible;
        Activate();
    }

    private void OnExitMenuItemClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (Default.HideAtStartup) Visibility = Visibility.Collapsed;

        NavigationView.Navigate(typeof(DetailsPage));

        // Update battery status when the computer resumes or when the power status changes.
        SystemEvents.PowerModeChanged += (_, e) =>
        {
            if (e.Mode is PowerModes.Resume or PowerModes.StatusChange) UpdateBatteryStatus();
        };

        // Update battery status when the display settings change.
        // This will redraw the tray icon to ensure optimal icon resolution under the current display settings.
        SystemEvents.DisplaySettingsChanged += (_, _) => UpdateBatteryStatus();

        // This event can be triggered multiple times when Windows changes between dark and light theme.
        // Update tray icon colour when user preference changes settled down.
        _userPreferenceChangedSubscription = Observable
            .FromEventPattern<UserPreferenceChangedEventHandler, UserPreferenceChangedEventArgs>(
                handler => SystemEvents.UserPreferenceChanged += handler,
                handler => SystemEvents.UserPreferenceChanged -= handler)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(AsyncOperationManager.SynchronizationContext)
            .Subscribe(_ => UpdateBatteryStatus());

        // Handle user settings change with debouncing.
        _userSettingChangedSubscription = Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => Default.PropertyChanged += handler,
                handler => Default.PropertyChanged -= handler)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(AsyncOperationManager.SynchronizationContext)
            .Subscribe(pattern => OnUserSettingsPropertyChanged(pattern.EventArgs.PropertyName));

        // Initial updates.
        UpdateBatteryStatus();
        UpdateRefreshSubscription();

        // Setup timer to update the tray icon.
        _batteryStatusRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Default.RefreshSeconds) };
        _batteryStatusRefreshTimer.Tick += (_, _) => UpdateBatteryStatus();
        _batteryStatusRefreshTimer.Start();
    }

    private void OnSettingsMenuItemClick(object sender, RoutedEventArgs e)
    {
        NavigationView.Navigate(typeof(SettingsPage));
        Visibility = Visibility.Visible;
        Activate();
    }

    private void OnUserSettingsPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Default.RefreshSeconds):
                _batteryStatusRefreshTimer.Interval = TimeSpan.FromSeconds(Default.RefreshSeconds);
                break;
            case nameof(Default.BatteryCriticalNotificationValue):
                if (Default.BatteryLowNotificationValue < Default.BatteryCriticalNotificationValue)
                    Default.BatteryLowNotificationValue = Default.BatteryCriticalNotificationValue;
                if (Default.BatteryHighNotificationValue < Default.BatteryCriticalNotificationValue)
                    Default.BatteryHighNotificationValue = Default.BatteryCriticalNotificationValue;
                break;
            case nameof(Default.BatteryLowNotificationValue):
                if (Default.BatteryCriticalNotificationValue > Default.BatteryLowNotificationValue)
                    Default.BatteryCriticalNotificationValue = Default.BatteryLowNotificationValue;
                if (Default.BatteryHighNotificationValue < Default.BatteryLowNotificationValue)
                    Default.BatteryHighNotificationValue = Default.BatteryLowNotificationValue;
                break;
            case nameof(Default.BatteryHighNotificationValue):
                if (Default.BatteryCriticalNotificationValue > Default.BatteryHighNotificationValue)
                    Default.BatteryCriticalNotificationValue = Default.BatteryHighNotificationValue;
                if (Default.BatteryLowNotificationValue > Default.BatteryHighNotificationValue)
                    Default.BatteryLowNotificationValue = Default.BatteryHighNotificationValue;
                break;
        }

        UpdateBatteryStatus();
    }

    private void SetNotifyIconText(string text, Brush foreground, string fontFamily = null)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            Foreground = foreground,
            FontSize = 18,
            Margin = new Thickness(-1.2)
        };

        if (fontFamily != null) textBlock.FontFamily = new FontFamily(fontFamily);
        else if (Default.TrayIconFontFamily != null) textBlock.FontFamily = Default.TrayIconFontFamily;

        if (Default.TrayIconFontBold) textBlock.FontWeight = FontWeights.Bold;

        if (Default.TrayIconFontUnderline) textBlock.TextDecorations = TextDecorations.Underline;

        NotifyIcon.Icon = GetImageSource(textBlock);
    }

    private void UpdateBatteryStatus()
    {
        var powerStatus = SystemInformation.PowerStatus;
        var batteryChargeStatus = powerStatus.BatteryChargeStatus;
        var percent = (int)Math.Round(powerStatus.BatteryLifePercent * 100);
        var notificationType = NotificationType.None;
        Brush brush;
        string trayIconText;
        switch (batteryChargeStatus)
        {
            case BatteryChargeStatus.NoSystemBattery:
                // When no battery detected.
                trayIconText = "❌";
                brush = GetNormalBrush();
                _notificationTitle = null;
                _notificationText = "No battery detected";
                // notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                break;
            case BatteryChargeStatus.Unknown:
                // When battery status is unknown.
                trayIconText = "❓";
                brush = GetNormalBrush();
                break;
            case BatteryChargeStatus.High:
            case BatteryChargeStatus.Low:
            case BatteryChargeStatus.Critical:
            case BatteryChargeStatus.Charging:
            default:
            {
                if (percent == 100)
                {
                    SetNotifyIconText("\uf5fc", GetNormalBrush(), "Segoe Fluent Icons");

                    var powerLineText = powerStatus.PowerLineStatus == PowerLineStatus.Online
                        ? " and connected to power"
                        : null;

                    NotifyIcon.TooltipText = _notificationText = "Your battery is fully charged" + powerLineText;

                    // If we don't need to show a fully charged notification, we can return straight away.
                    if (!Default.BatteryFullNotification) return;

                    // Show fully charged notification.

                    _notificationTitle = "Fully charged" + powerLineText;
                    notificationType = NotificationType.Full;
                    CheckAndSendNotification();

                    return;
                }

                // When battery status is normal, display percentage in tray icon.
                trayIconText = percent.ToString();
                if (batteryChargeStatus.HasFlag(BatteryChargeStatus.Charging))
                {
                    // When the battery is charging.
                    brush = GetBrushFromColourHexString(Default.BatteryChargingColour,
                        (Color)ColorConverter.ConvertFromString(App.DefaultBatteryChargingColour)!);
                    // notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    var report = Battery.AggregateBattery.GetReport();
                    var chargeRateInMilliWatts = report.ChargeRateInMilliwatts;
                    if (chargeRateInMilliWatts > 0)
                    {
                        _notificationTitle = percent + "% charging";

                        var fullChargeCapacityInMilliWattHours = report.FullChargeCapacityInMilliwattHours;
                        var remainingCapacityInMilliWattHours = report.RemainingCapacityInMilliwattHours;
                        if (fullChargeCapacityInMilliWattHours.HasValue &&
                            remainingCapacityInMilliWattHours.HasValue)
                            _notificationText = Helper.GetReadableTimeSpan(TimeSpan.FromHours(
                                (fullChargeCapacityInMilliWattHours.Value -
                                 remainingCapacityInMilliWattHours.Value) /
                                (double)chargeRateInMilliWatts.Value)) + " until fully charged";
                    }
                    else
                    {
                        _notificationTitle = null;
                        _notificationText = percent + "% charging";
                    }

                    SetHighOrFullNotification();
                }
                else
                {
                    // When battery is not charging.
                    if (percent <= Default.BatteryCriticalNotificationValue)
                    {
                        // When battery capacity is critical.
                        brush = GetBrushFromColourHexString(Default.BatteryCriticalColour,
                            (Color)ColorConverter.ConvertFromString(App.DefaultBatteryCriticalColour)!);
                        // notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                        if (Default.BatteryCriticalNotification) notificationType = NotificationType.Critical;
                    }
                    else if (percent <= Default.BatteryLowNotificationValue)
                    {
                        // When battery capacity is low.
                        brush = GetBrushFromColourHexString(Default.BatteryLowColour,
                            (Color)ColorConverter.ConvertFromString(App.DefaultBatteryLowColour)!);
                        // notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
                        if (Default.BatteryLowNotification) notificationType = NotificationType.Low;
                    }
                    else
                    {
                        // When battery capacity is normal.
                        brush = GetNormalBrush();
                        // notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                        SetHighOrFullNotification();
                    }

                    if (powerStatus.BatteryLifeRemaining > 0)
                    {
                        _notificationTitle = $"{percent}% {(powerStatus.PowerLineStatus == PowerLineStatus.Online
                            ? "connected (not charging)"
                            : "on battery")}";
                        _notificationText =
                            Helper.GetReadableTimeSpan(TimeSpan.FromSeconds(powerStatus.BatteryLifeRemaining)) +
                            " remaining";
                    }
                    else
                    {
                        _notificationTitle = null;
                        _notificationText =
                            $"{percent}% {(powerStatus.PowerLineStatus == PowerLineStatus.Online
                                ? "connected (not charging)"
                                : "on battery")}";
                    }
                }

                break;

                void SetHighOrFullNotification()
                {
                    if (percent == Default.BatteryHighNotificationValue && Default.BatteryHighNotification)
                        notificationType = NotificationType.High;
                    else if (percent == 100 && Default.BatteryFullNotification)
                        notificationType = NotificationType.Full;
                }
            }
        }

        // Set tray icon tool tip based on the balloon notification texts.
        NotifyIcon.TooltipText = _notificationTitle == null
            ? _notificationText
            : _notificationTitle + Environment.NewLine + _notificationText;

        SetNotifyIconText(trayIconText, brush);

        CheckAndSendNotification();
        return;

        // Check and send notification.
        void CheckAndSendNotification()
        {
            if (notificationType == NotificationType.None)
                // No notification required.
                return;

            var utcNow = DateTime.UtcNow;
            if (_lastNotification.Type != notificationType)
                // Notification required, and battery status has changed from last notification, notify user.
                new ToastContentBuilder()
                    .AddText(_notificationTitle)
                    .AddText(_notificationText)
                    .Show();
            else if (utcNow - _lastNotification.DateTime > TimeSpan.FromMinutes(5))
                // Notification required, but battery status is the same as last notification for more than 5 minutes,
                // notify user.
                new ToastContentBuilder()
                    .AddText(_notificationTitle)
                    .AddText(_notificationText)
                    .Show();

            _lastNotification = (notificationType, utcNow);
        }
    }

    private void UpdateRefreshSubscription()
    {
        _refreshSubscription?.Dispose();
        _refreshSubscription = Observable.Interval(TimeSpan.FromSeconds(Default.RefreshSeconds))
            .ObserveOn(AsyncOperationManager.SynchronizationContext).Subscribe(_ => UpdateBatteryStatus());
    }
}