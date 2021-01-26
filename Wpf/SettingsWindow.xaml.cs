using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.ApplicationModel;
using Percentage.Wpf.Properties;
using Button = System.Windows.Controls.Button;
using Color = System.Drawing.Color;
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

        protected override void OnContentRendered(EventArgs e)
        {
            RegisterEventHandling();
            base.OnContentRendered(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            UnRegisterEventHandling();
            UnRegisterAutoStarEventHandling();
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

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            UnRegisterEventHandling();
            Default.LowColor = Color.FromArgb(202, 80, 16);
            Default.CriticalColor = Color.FromArgb(232, 17, 35);
            Default.ChargingColor = Color.FromArgb(16, 137, 62);
            Default.NormalColor = Color.Transparent;
            CriticalNotification.IsChecked = LowNotification.IsChecked =
                HighNotification.IsChecked = FullNotification.IsChecked = true;
            HighNotificationValue.SelectedItem = 80;
            LowNotificationValue.SelectedItem = 20;
            CriticalNotificationValue.SelectedItem = 10;
            RefreshSeconds.SelectedItem = 10;
            TrayIconFontName.SelectedValue = System.Drawing.FontFamily.GenericSansSerif.Name;
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