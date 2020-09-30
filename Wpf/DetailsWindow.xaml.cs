using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Windows.Devices.Power;
using Windows.System.Power;

namespace Percentage.Wpf
{
    public partial class DetailsWindow
    {
        public DetailsWindow()
        {
            InitializeComponent();

            Update();

            var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(10)};
            timer.Tick += (sender, args) => Update();
            timer.Start();

            void Update()
            {
                var report = Battery.AggregateBattery.GetReport();
                Percentage.Text = report.Status == BatteryStatus.NotPresent
                    ? "Unknown"
                    : SystemInformation.PowerStatus.BatteryLifePercent.ToString("P");
                ChargeRate.Text = report.ChargeRateInMilliwatts == null
                    ? "Unknown"
                    : report.ChargeRateInMilliwatts == 0
                        ? "Not Charging"
                        : report.ChargeRateInMilliwatts + " mW";
                var designCapacity = report.DesignCapacityInMilliwattHours;
                DesignCapacity.Text = designCapacity == null
                    ? "Unknown"
                    : designCapacity + " mWh";
                var fullChargeCapacity = report.FullChargeCapacityInMilliwattHours;
                FullChargeCapacity.Text = fullChargeCapacity == null
                    ? "Unknown"
                    : fullChargeCapacity + " mWh";
                RemainingChargeCapacity.Text = report.RemainingCapacityInMilliwattHours == null
                    ? "Unknown"
                    : report.RemainingCapacityInMilliwattHours + " mWh";
                if (designCapacity != null && fullChargeCapacity != null)
                {
                    var health = (double) fullChargeCapacity.Value / designCapacity.Value;
                    Health.Text = (health > 1 ? 1 : health).ToString("P");
                }
                else
                {
                    Health.Text = "Unknown";
                }

                Status.Text = report.Status.ToString().CamelCaseSplit();
            }
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Helper.ShowRatingView();
        }
    }
}