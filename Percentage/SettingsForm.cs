using System;
using System.Drawing;
using System.Windows.Forms;
using Percentage.Properties;

namespace Percentage
{
    public partial class SettingsForm : Form
    {
        private static readonly Color ChargingColor = Color.FromArgb(16, 137, 62);
        private static readonly Color LowColor = Color.FromArgb(202, 80, 16);
        private static readonly Color CriticalColor = Color.FromArgb(232, 17, 35);
        private static readonly Settings Settings = Settings.Default;

        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LowColorButton.BackColor = Settings.LowColor;
            CriticalColorButton.BackColor = Settings.CriticalColor;
            ChargingColorButton.BackColor = Settings.ChargingColor;
            CriticalNotificationCheckBox.Checked = Settings.CriticalNotification;
            LowNotificationCheckBox.Checked = Settings.LowNotification;
            HighNotificationCheckBox.Checked = Settings.HighNotification;
            FullNotificationCheckBox.Checked = Settings.FullNotification;
            AutoStartCheckBox.Checked = Settings.AutoStart;

            LowColorButton.Click += ButtonClick;
            CriticalColorButton.Click += ButtonClick;
            ChargingColorButton.Click += ButtonClick;
            CriticalNotificationCheckBox.CheckedChanged += CheckBoxCheckedChanged;
            LowNotificationCheckBox.CheckedChanged += CheckBoxCheckedChanged;
            HighNotificationCheckBox.CheckedChanged += CheckBoxCheckedChanged;
            FullNotificationCheckBox.CheckedChanged += CheckBoxCheckedChanged;
            AutoStartCheckBox.CheckedChanged += CheckBoxCheckedChanged;

            ResetButton.Click += (_, __) =>
            {
                LowColorButton.BackColor = Settings.LowColor = LowColor;
                CriticalColorButton.BackColor = Settings.CriticalColor = CriticalColor;
                ChargingColorButton.BackColor = Settings.ChargingColor = ChargingColor;
                CriticalNotificationCheckBox.Checked = LowNotificationCheckBox.Checked =
                    HighNotificationCheckBox.Checked = FullNotificationCheckBox.Checked = AutoStartCheckBox.Checked =
                        Settings.CriticalNotification = Settings.LowNotification = Settings.FullNotification =
                            Settings.HighNotification = Settings.AutoStart = true;
                Settings.Save();
            };

            static void CheckBoxCheckedChanged(object sender, EventArgs _)
            {
                var checkBox = (CheckBox) sender;
                Settings[(string) checkBox.Tag] = checkBox.Checked;
                Settings.Save();
            }

            static void ButtonClick(object sender, EventArgs e)
            {
                var button = (Button) sender;
                var dialog = new ColorDialog
                {
                    Color = button.BackColor,
                    AllowFullOpen = true,
                    AnyColor = true,
                    FullOpen = true
                };
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                Settings[(string) button.Tag] = button.BackColor = dialog.Color;
                Settings.Save();
            }
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.Escape)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            Close();
            return true;
        }
    }
}