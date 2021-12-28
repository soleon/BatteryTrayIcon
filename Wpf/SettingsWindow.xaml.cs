using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.ApplicationModel;
using Percentage.Wpf.Properties;
using Button = System.Windows.Controls.Button;
using Color = System.Drawing.Color;
using MessageBox = System.Windows.MessageBox;

namespace Percentage.Wpf;

using static Settings;

public partial class SettingsWindow
{
    private StartupTask _startupTask;

    public SettingsWindow()
    {
        InitializeComponent();
        new Func<Task>(async () =>
        {
            try
            {
                AutoStart.IsChecked = (_startupTask = await StartupTask.GetAsync(Program.Id)).State ==
                                      StartupTaskState.Enabled;
            }
            catch
            {
                AutoStartDisabledText.Visibility = Visibility.Visible;
                return;
            }

            RegisterAutoStartEventHandling();
            AutoStart.IsEnabled = true;
        })();
    }

    private static void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Default.Save();
    }

    private static void RegisterEventHandling()
    {
        Default.PropertyChanged += OnSettingPropertyChanged;
    }

    private static void UnRegisterEventHandling()
    {
        Default.PropertyChanged -= OnSettingPropertyChanged;
    }

    private async void EnableAutoStart()
    {
        if (_startupTask == null)
        {
            return;
        }

        AutoStart.IsEnabled = false;
        var state = await _startupTask.RequestEnableAsync();
        UnRegisterAutoStarEventHandling();
        switch (state)
        {
            case StartupTaskState.Disabled:
                AutoStart.IsChecked = false;
                MessageBox.Show(
                    "Auto start for this app has been disabled.\r\n\r\nTo re-enable it, please go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\".",
                    "Auto start disabled", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
            case StartupTaskState.DisabledByUser:
                AutoStart.IsChecked = false;
                MessageBox.Show(
                    "Auto start for this app has been disabled manually in system settings.\r\n\r\nTo re-enable it, please go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\".",
                    "Auto start disabled by user", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
            case StartupTaskState.DisabledByPolicy:
                AutoStart.IsChecked = false;
                MessageBox.Show(
                    "Auto start for this app has been disabled manually by system policy.\r\n\r\nTo re-enable it, remove any system policies that restrict auto start, and go to \"Settings > Apps > Startup\" to enable \"Battery Percentage Icon\".",
                    "Auto start disabled by policy", MessageBoxButton.OK, MessageBoxImage.Warning);
                break;
            case StartupTaskState.Enabled:
            case StartupTaskState.EnabledByPolicy:
                AutoStart.IsChecked = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        RegisterAutoStartEventHandling();
        AutoStart.IsEnabled = true;
    }

    private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
    {
        Helper.ShowRatingView();
    }

    private void OnAutoStartChecked(object sender, RoutedEventArgs e)
    {
        EnableAutoStart();
    }

    private void OnAutoStartUnchecked(object sender, RoutedEventArgs e)
    {
        _startupTask.Disable();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        UnRegisterEventHandling();
        UnRegisterAutoStarEventHandling();
    }

    private void OnColorButtonClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var mediaColor = ((SolidColorBrush)button.Background).Color;
        var dialog = new ColorDialog
        {
            Color = Color.FromArgb(mediaColor.R, mediaColor.G, mediaColor.B),
            AllowFullOpen = true,
            AnyColor = true,
            FullOpen = true
        };
        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        {
            return;
        }

        Default[(string)button.Tag] = dialog.Color;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        RegisterEventHandling();
        base.OnContentRendered(e);
    }

    private void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.SendFeedBack();
    }

    private void OnResetButtonClick(object sender, RoutedEventArgs e)
    {
        UnRegisterEventHandling();
        Default.LowColor = Color.FromArgb(202, 80, 16);
        Default.CriticalColor = Color.FromArgb(232, 17, 35);
        Default.ChargingColor = Color.FromArgb(16, 137, 62);
        Default.NormalColor = Color.Transparent;
        Default.CriticalNotification = Default.LowNotification =
            Default.HighNotification = Default.FullNotification = true;
        Default.HighNotificationValue = 80;
        Default.LowNotificationValue = 20;
        Default.CriticalNotificationValue = 10;
        Default.RefreshSeconds = 10;
        using (Default.TrayIconFont)
        {
            Default.TrayIconFont = new Font(System.Drawing.FontFamily.GenericSansSerif, 11);
        }

        Default.Save();
        RegisterEventHandling();
        EnableAutoStart();
    }

    private void RegisterAutoStartEventHandling()
    {
        AutoStart.Checked += OnAutoStartChecked;
        AutoStart.Unchecked += OnAutoStartUnchecked;
    }

    private void UnRegisterAutoStarEventHandling()
    {
        AutoStart.Checked -= OnAutoStartChecked;
        AutoStart.Unchecked -= OnAutoStartUnchecked;
    }
}