using System;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Wpf.Ui.Controls;
using MessageBox = Wpf.Ui.Controls.MessageBox;
using static Percentage.App.Properties.Settings;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace Percentage.App;

public partial class SettingsPage
{
    private StartupTask _startupTask;

    public SettingsPage()
    {
        InitializeComponent();

        new Func<Task>(async () =>
        {
            try
            {
                AutoStartToggleSwitch.IsChecked = (_startupTask = await StartupTask.GetAsync(Program.Id)).State ==
                                                  StartupTaskState.Enabled;
            }
            catch
            {
                AutoStartDisabledInfoBar.Visibility = Visibility.Visible;
                return;
            }

            RegisterAutoStartEventHandling();
            AutoStartToggleSwitch.IsEnabled = true;
        })();
    }

    private async Task EnableAutoStart()
    {
        if (_startupTask == null) return;

        AutoStartToggleSwitch.IsEnabled = false;
        var state = await _startupTask.RequestEnableAsync();
        UnRegisterAutoStarEventHandling();
        switch (state)
        {
            case StartupTaskState.Disabled:
                AutoStartToggleSwitch.IsChecked = false;
                await new MessageBox
                {
                    Title = "Auto start disabled",
                    Content = "Auto start for this app has been disabled.\r\n\r\n" +
                              "To re-enable it, please go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\"."
                }.ShowDialogAsync();
                break;
            case StartupTaskState.DisabledByUser:
                AutoStartToggleSwitch.IsChecked = false;
                await new MessageBox
                {
                    Title = "Auto start disabled by user",
                    Content = "Auto start for this app has been disabled manually in system settings.\r\n\r\n" +
                              "To re-enable it, please go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\"."
                }.ShowDialogAsync();
                break;
            case StartupTaskState.DisabledByPolicy:
                AutoStartToggleSwitch.IsChecked = false;
                await new MessageBox
                {
                    Title = "Auto start disabled by policy",
                    Content = "Auto start for this app has been disabled manually by system policy.\r\n\r\n" +
                              "To re-enable it, remove any system policies that restrict auto start, and go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\"."
                }.ShowDialogAsync();
                break;
            case StartupTaskState.Enabled:
            case StartupTaskState.EnabledByPolicy:
                AutoStartToggleSwitch.IsChecked = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        RegisterAutoStartEventHandling();
        AutoStartToggleSwitch.IsEnabled = true;
    }

    private void OnAutoStartChecked(object sender, RoutedEventArgs e)
    {
        _ = EnableAutoStart();
    }

    private void OnAutoStartUnchecked(object sender, RoutedEventArgs e)
    {
        _startupTask.Disable();
    }

    private void RegisterAutoStartEventHandling()
    {
        AutoStartToggleSwitch.Checked += OnAutoStartChecked;
        AutoStartToggleSwitch.Unchecked += OnAutoStartUnchecked;
    }

    private void UnRegisterAutoStarEventHandling()
    {
        AutoStartToggleSwitch.Checked -= OnAutoStartChecked;
        AutoStartToggleSwitch.Unchecked -= OnAutoStartUnchecked;
    }

    private void OnResetButtonClick(object sender, RoutedEventArgs e)
    {
        var result = new MessageBox
        {
            Title = "Reset All Settings",
            Content = "Are you sure you want to reset all settings to their default values?",
            IsPrimaryButtonEnabled = true,
            PrimaryButtonText = "Reset",
            PrimaryButtonAppearance = ControlAppearance.Caution,
            CloseButtonText = "Cancel"
        }.ShowDialogAsync().GetAwaiter().GetResult();

        if (result != MessageBoxResult.Primary)
        {
            return;
        }
        
        Default.BatteryCriticalColour = ApplicationDefault.BatteryCriticalColour;
        Default.BatteryLowColour = ApplicationDefault.BatteryLowColour;
        Default.BatteryChargingColour = ApplicationDefault.BatteryChargingColour;
        Default.BatteryNormalColour = ApplicationDefault.BatteryNormalColour;
        Default.TrayIconFontFamily = ApplicationDefault.TrayIconFontFamily;
        Default.TrayIconFontBold = ApplicationDefault.TrayIconFontBold;
        Default.TrayIconFontUnderline = ApplicationDefault.TrayIconFontUnderline;
        Default.BatteryCriticalNotificationValue = ApplicationDefault.BatteryCriticalNotificationValue;
        Default.BatteryLowNotificationValue = ApplicationDefault.BatteryLowNotificationValue;
        Default.BatteryHighNotificationValue = ApplicationDefault.BatteryHighNotificationValue;
        Default.RefreshSeconds = ApplicationDefault.RefreshSeconds;
        Default.BatteryFullNotification = ApplicationDefault.BatteryFullNotification;
        Default.BatteryLowNotification = ApplicationDefault.BatteryLowNotification;
        Default.BatteryHighNotification = ApplicationDefault.BatteryHighNotification;
        Default.BatteryCriticalNotification = ApplicationDefault.BatteryCriticalNotification;
        Default.HideAtStartup = ApplicationDefault.HideAtStartup;
        
        Default.Save();
        
        _ = EnableAutoStart();
    }
}