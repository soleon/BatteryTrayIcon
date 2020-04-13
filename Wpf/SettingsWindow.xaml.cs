using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using static Wpf.Helper;
using static Wpf.Properties.Settings;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using Color = System.Drawing.Color;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Wpf
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LowColor.Background = new SolidColorBrush(Default.LowColor.ToMediaColor());
            CriticalColor.Background = new SolidColorBrush(Default.CriticalColor.ToMediaColor());
            ChargingColor.Background = new SolidColorBrush(Default.ChargingColor.ToMediaColor());
            CriticalNotification.IsChecked = Default.CriticalNotification;
            LowNotification.IsChecked = Default.LowNotification;
            HighNotification.IsChecked = Default.HighNotification;
            FullNotification.IsChecked = Default.FullNotification;
            AutoStart.IsChecked = IsAutoStart();
            HighNotificationValue.SelectedItem = Default.HighNotificationValue;
            RefreshSeconds.SelectedItem = Default.RefreshSeconds;
            RegisterEventHandling();
        }

        private void RegisterEventHandling()
        {
            CriticalNotification.Checked += OnCheckBoxChecked;
            CriticalNotification.Unchecked += OnCheckedBoxUnChecked;
            LowNotification.Checked += OnCheckBoxChecked;
            LowNotification.Unchecked += OnCheckedBoxUnChecked;
            HighNotification.Checked += OnCheckBoxChecked;
            HighNotification.Unchecked += OnCheckedBoxUnChecked;
            FullNotification.Checked += OnCheckBoxChecked;
            FullNotification.Unchecked += OnCheckedBoxUnChecked;
            AutoStart.Checked += OnAutoStartChecked;
            AutoStart.Unchecked += OnAutoStartUnchecked;
            HighNotificationValue.SelectionChanged += OnComboBoxSelectionChanged;
            RefreshSeconds.SelectionChanged += OnComboBoxSelectionChanged;
        }

        private void UnRegisterEventHandling()
        {
            CriticalNotification.Checked -= OnCheckBoxChecked;
            CriticalNotification.Unchecked -= OnCheckedBoxUnChecked;
            LowNotification.Checked -= OnCheckBoxChecked;
            LowNotification.Unchecked -= OnCheckedBoxUnChecked;
            HighNotification.Checked -= OnCheckBoxChecked;
            HighNotification.Unchecked -= OnCheckedBoxUnChecked;
            FullNotification.Checked -= OnCheckBoxChecked;
            FullNotification.Unchecked -= OnCheckedBoxUnChecked;
            AutoStart.Checked -= OnAutoStartChecked;
            AutoStart.Unchecked -= OnAutoStartUnchecked;
            HighNotificationValue.SelectionChanged -= OnComboBoxSelectionChanged;
            RefreshSeconds.SelectionChanged -= OnComboBoxSelectionChanged;
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

            Default[button.Name] = dialog.Color;
            Default.Save();
            button.Background = new SolidColorBrush(dialog.Color.ToMediaColor());
        }

        private static void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            Default[checkBox.Name] = true;
            Default.Save();
        }

        private static void OnCheckedBoxUnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox) sender;
            Default[checkBox.Name] = false;
            Default.Save();
        }

        private static void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox) sender;
            Default[comboBox.Name] = comboBox.SelectedItem;
            Default.Save();
        }

        private void OnResetButtonCLick(object sender, RoutedEventArgs e)
        {
            UnRegisterEventHandling();
            LowColor.Background =
                new SolidColorBrush((Default.LowColor = Color.FromArgb(202, 80, 16)).ToMediaColor());
            CriticalColor.Background =
                new SolidColorBrush((Default.CriticalColor = Color.FromArgb(232, 17, 35)).ToMediaColor());
            ChargingColor.Background =
                new SolidColorBrush((Default.ChargingColor = Color.FromArgb(16, 137, 62)).ToMediaColor());
            CriticalNotification.IsChecked = LowNotification.IsChecked =
                HighNotification.IsChecked = FullNotification.IsChecked = AutoStart.IsChecked =
                    Default.CriticalNotification = Default.LowNotification = Default.FullNotification =
                        Default.HighNotification = true;
            HighNotificationValue.SelectedItem = Default.HighNotificationValue = 80;
            RefreshSeconds.SelectedItem = Default.RefreshSeconds = 10;
            Default.Save();
            EnableAutoStart();
            RegisterEventHandling();
        }

        private void OnAutoStartChecked(object sender, RoutedEventArgs e)
        {
            EnableAutoStart();
        }

        private void OnAutoStartUnchecked(object sender, RoutedEventArgs e)
        {
            DisableAutoRun();
        }
    }
}