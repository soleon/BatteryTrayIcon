using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.ApplicationModel;
using Percentage.Wpf.Properties;
using Button = System.Windows.Controls.Button;
using Cursors = System.Windows.Input.Cursors;

namespace Percentage.Wpf
{
    using static Settings;

    public partial class SettingsWindow
    {
        private StartupTask _startupTask;

        public SettingsWindow()
        {
            InitializeComponent();
            LowColor.Background = new SolidColorBrush(ToMediaColor(Default.LowColor));
            CriticalColor.Background = new SolidColorBrush(ToMediaColor(Default.CriticalColor));
            ChargingColor.Background = new SolidColorBrush(ToMediaColor(Default.ChargingColor));
            RegisterEventHandling();
            new Action(async () =>
            {
                try
                {
                    AutoStart.IsChecked = (_startupTask = await StartupTask.GetAsync(Program.Id)).State ==
                                          StartupTaskState.Enabled;
                }
                catch (Exception e)
                {
                    AutoStart.IsEnabled = true;
                    AutoStart.Cursor = Cursors.No;
                    AutoStart.Opacity = 0.3;
                    AutoStart.ToolTip = "Auto start requires this app to run as UWP.\r\n\r\n" + e.Message;
                    return;
                }

                RegisterAutoStartEventHandling();
                AutoStart.IsEnabled = true;
            })();
        }

        private static void RegisterEventHandling()
        {
            Default.PropertyChanged += OnSettingPropertyChanged;
        }

        private static void OnSettingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Default.Save();
        }

        private static void UnRegisterEventHandling()
        {
            Default.PropertyChanged -= OnSettingPropertyChanged;
        }

        private void OnColorButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var mediaColor = ((SolidColorBrush) button.Background).Color;
            var dialog = new ColorDialog
            {
                Color = System.Drawing.Color.FromArgb(mediaColor.R, mediaColor.G, mediaColor.B),
                AllowFullOpen = true,
                AnyColor = true,
                FullOpen = true
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            Default[button.Name] = dialog.Color;
            Default.Save();
            button.Background = new SolidColorBrush(ToMediaColor(dialog.Color));
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            UnRegisterEventHandling();
            LowColor.Background =
                new SolidColorBrush(ToMediaColor(Default.LowColor = System.Drawing.Color.FromArgb(202, 80, 16)));
            CriticalColor.Background =
                new SolidColorBrush(ToMediaColor(Default.CriticalColor = System.Drawing.Color.FromArgb(232, 17, 35)));
            ChargingColor.Background =
                new SolidColorBrush(ToMediaColor(Default.ChargingColor = System.Drawing.Color.FromArgb(16, 137, 62)));
            CriticalNotification.IsChecked = LowNotification.IsChecked =
                HighNotification.IsChecked = FullNotification.IsChecked = true;
            HighNotificationValue.SelectedItem = 80;
            LowNotificationValue.SelectedItem = 20;
            CriticalNotificationValue.SelectedItem = 10;
            RefreshSeconds.SelectedItem = 10;
            TrayIconFontName.SelectedValue = SystemFonts.IconFontFamily.Source;
            Default.Save();
            RegisterEventHandling();
            EnableAutoStart();
        }

        private void OnAutoStartChecked(object sender, RoutedEventArgs e)
        {
            EnableAutoStart();
        }

        private void OnAutoStartUnchecked(object sender, RoutedEventArgs e)
        {
            _startupTask.Disable();
        }

        private async void EnableAutoStart()
        {
            AutoStart.IsEnabled = false;
            var state = await _startupTask.RequestEnableAsync();
            UnRegisterAutoStarEventHandling();
            switch (state)
            {
                case StartupTaskState.Disabled:
                case StartupTaskState.DisabledByUser:
                case StartupTaskState.DisabledByPolicy:
                    AutoStart.IsChecked = false;
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

        private void UnRegisterAutoStarEventHandling()
        {
            AutoStart.Checked -= OnAutoStartChecked;
            AutoStart.Unchecked -= OnAutoStartUnchecked;
        }

        private void RegisterAutoStartEventHandling()
        {
            AutoStart.Checked += OnAutoStartChecked;
            AutoStart.Unchecked += OnAutoStartUnchecked;
        }

        private static Color ToMediaColor(System.Drawing.Color color)
        {
            return Color.FromRgb(color.R, color.G, color.B);
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Helper.ShowRatingView();
        }

        private void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            Helper.SendFeedBack();
        }
    }
}