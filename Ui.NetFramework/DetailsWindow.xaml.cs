using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.Devices.Power;
using Windows.System.Power;
using Percentage.Ui.NetFramework.Properties;

namespace Percentage.Ui.NetFramework;

using static Settings;

public partial class DetailsWindow
{
    public DetailsWindow()
    {
        InitializeComponent();

        Update();

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Default.RefreshSeconds) };
        Default.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(Default.RefreshSeconds))
            {
                timer.Interval = TimeSpan.FromSeconds(Default.RefreshSeconds);
            }
        };
        timer.Tick += (_, _) => Update();
        timer.Start();

        void Update()
        {
            var report = Battery.AggregateBattery.GetReport();
            var powerStatus = SystemInformation.PowerStatus;
            Percentage.Text = report.Status == BatteryStatus.NotPresent
                ? "Unknown"
                : powerStatus.BatteryLifePercent.ToString("P");
            var chargeRateInMilliwatts = report.ChargeRateInMilliwatts;
            var fullChargeCapacityInMilliwattHours = report.FullChargeCapacityInMilliwattHours;
            var remainingCapacityInMilliwattHours = report.RemainingCapacityInMilliwattHours;
            switch (chargeRateInMilliwatts)
            {
                case null:
                    TimeLabel.Text = "Remaining Time";
                    ChargeRate.Text = TimeValue.Text = "Unknown";
                    break;
                case 0:
                    TimeLabel.Text = "Remaining Time";
                    TimeValue.Text = "Unknown";
                    ChargeRate.Text = "Not Charging";
                    break;
                default:
                    if (chargeRateInMilliwatts > 0)
                    {
                        TimeLabel.Text = "Time Until Full";
                        if (fullChargeCapacityInMilliwattHours.HasValue && remainingCapacityInMilliwattHours.HasValue)
                        {
                            TimeValue.Text = Helper.GetReadableTimeSpan(TimeSpan.FromHours(
                                (fullChargeCapacityInMilliwattHours.Value -
                                 remainingCapacityInMilliwattHours.Value) /
                                (double)chargeRateInMilliwatts.Value));
                        }
                        else
                        {
                            TimeValue.Text = "Unknown";
                        }
                    }
                    else
                    {
                        TimeLabel.Text = "Remaining Time";
                        TimeValue.Text =
                            Helper.GetReadableTimeSpan(TimeSpan.FromSeconds(powerStatus.BatteryLifeRemaining));
                    }

                    ChargeRate.Text = chargeRateInMilliwatts + " mW";
                    break;
            }

            PowerLineStatus.Text = powerStatus.PowerLineStatus switch
            {
                System.Windows.Forms.PowerLineStatus.Online => "Connected",
                System.Windows.Forms.PowerLineStatus.Offline => "Disconnected",
                _ => "Unknown"
            };
            var designCapacity = report.DesignCapacityInMilliwattHours;
            DesignCapacity.Text = designCapacity == null
                ? "Unknown"
                : designCapacity + " mWh";
            FullChargeCapacity.Text = fullChargeCapacityInMilliwattHours == null
                ? "Unknown"
                : fullChargeCapacityInMilliwattHours + " mWh";
            RemainingChargeCapacity.Text = remainingCapacityInMilliwattHours == null
                ? "Unknown"
                : remainingCapacityInMilliwattHours + " mWh";
            if (designCapacity != null && fullChargeCapacityInMilliwattHours != null)
            {
                var health = (double)fullChargeCapacityInMilliwattHours.Value / designCapacity.Value;
                Health.Text = (health > 1 ? 1 : health).ToString("P");
            }
            else
            {
                Health.Text = "Unknown";
            }

            Status.Text = Regex.Replace(report.Status.ToString(), @"\B[A-Z]", " $0");
        }
    }

    internal event Action SettingsWindowRequested;

    private void OnFeedbackClick(object sender, RoutedEventArgs e)
    {
        Helper.SendFeedBack();
    }

    private void OnRatingClick(object sender, RoutedEventArgs e)
    {
        Helper.ShowRatingView();
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        SettingsWindowRequested?.Invoke();
    }
}