using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Devices.Power;
using Windows.System.Power;
using Wpf.Ui.Controls;
using Wpf.Ui.Markup;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using PowerLineStatus = System.Windows.Forms.PowerLineStatus;
using Size = System.Windows.Size;

namespace Percentage.App;

public partial class DetailsPage : INotifyPropertyChanged
{
    private string _batterLifePercentText;
    private string _batteryHealthText;
    private string _batteryStatusText;
    private string _chargeRateText;
    private string _designCapacityText;
    private string _fullChargeCapacityText;
    private string _powerLineStatusText;
    private string _remainingChargeCapacityText;
    private string _timeLabelText;
    private string _timeValueText;

    public DetailsPage()
    {
        InitializeComponent();

        IDisposable updateSubscription = null;
        Loaded += (_, _) =>
        {
            Update();
            updateSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOn(AsyncOperationManager.SynchronizationContext).Subscribe(_ => Update());
        };

        Unloaded += (_, _) => { updateSubscription?.Dispose(); };
    }

    public string BatterLifePercentText
    {
        get => _batterLifePercentText;
        private set => SetField(ref _batterLifePercentText, value);
    }

    public string TimeLabelText
    {
        get => _timeLabelText;
        private set => SetField(ref _timeLabelText, value);
    }

    public string TimeValueText
    {
        get => _timeValueText;
        private set => SetField(ref _timeValueText, value);
    }

    public string ChargeRateText
    {
        get => _chargeRateText;
        private set => SetField(ref _chargeRateText, value);
    }

    public string PowerLineStatusText
    {
        get => _powerLineStatusText;
        private set => SetField(ref _powerLineStatusText, value);
    }

    public string BatteryStatusText
    {
        get => _batteryStatusText;
        private set => SetField(ref _batteryStatusText, value);
    }

    public string DesignCapacityText
    {
        get => _designCapacityText;
        private set => SetField(ref _designCapacityText, value);
    }

    public string FullChargeCapacityText
    {
        get => _fullChargeCapacityText;
        private set => SetField(ref _fullChargeCapacityText, value);
    }

    public string RemainingChargeCapacityText
    {
        get => _remainingChargeCapacityText;
        private set => SetField(ref _remainingChargeCapacityText, value);
    }

    public string BatteryHealthText
    {
        get => _batteryHealthText;
        private set => SetField(ref _batteryHealthText, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;


    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    [GeneratedRegex(@"\B[A-Z]")]
    private static partial Regex WordStartLetterRegex();

    private void Update()
    {
        var report = Battery.AggregateBattery.GetReport();
        var powerStatus = SystemInformation.PowerStatus;
        BatterLifePercentText = report.Status == BatteryStatus.NotPresent
            ? "Unknown"
            : powerStatus.BatteryLifePercent.ToString("P");
        var chargeRateInMilliWatts = report.ChargeRateInMilliwatts;
        var fullChargeCapacityInMilliWattHours = report.FullChargeCapacityInMilliwattHours;
        var remainingCapacityInMilliWattHours = report.RemainingCapacityInMilliwattHours;
        switch (chargeRateInMilliWatts)
        {
            case null:
                TimeLabelText = "Remaining Time";
                ChargeRateText = TimeValueText = "Unknown";
                break;
            case 0:
                TimeLabelText = "Remaining Time";
                TimeValueText = "Unknown";
                ChargeRateText = "Not Charging";
                break;
            default:
                if (chargeRateInMilliWatts > 0)
                {
                    TimeLabelText = "Time Until Full";
                    if (fullChargeCapacityInMilliWattHours.HasValue && remainingCapacityInMilliWattHours.HasValue)
                        TimeValueText = Helper.GetReadableTimeSpan(TimeSpan.FromHours(
                            (fullChargeCapacityInMilliWattHours.Value -
                             remainingCapacityInMilliWattHours.Value) /
                            (double)chargeRateInMilliWatts.Value));
                    else
                        TimeValueText = "Unknown";
                }
                else
                {
                    TimeLabelText = "Remaining Time";
                    TimeValueText =
                        Helper.GetReadableTimeSpan(TimeSpan.FromSeconds(powerStatus.BatteryLifeRemaining));
                }

                ChargeRateText = chargeRateInMilliWatts + " mW";
                break;
        }

        PowerLineStatusText = powerStatus.PowerLineStatus switch
        {
            PowerLineStatus.Online => "Connected",
            PowerLineStatus.Offline => "Disconnected",
            _ => "Unknown"
        };
        var designCapacity = report.DesignCapacityInMilliwattHours;
        DesignCapacityText = designCapacity == null
            ? "Unknown"
            : designCapacity + " mWh";
        FullChargeCapacityText = fullChargeCapacityInMilliWattHours == null
            ? "Unknown"
            : fullChargeCapacityInMilliWattHours + " mWh";
        RemainingChargeCapacityText = remainingCapacityInMilliWattHours == null
            ? "Unknown"
            : remainingCapacityInMilliWattHours + " mWh";
        if (designCapacity != null && fullChargeCapacityInMilliWattHours != null)
        {
            var health = (double)fullChargeCapacityInMilliWattHours.Value / designCapacity.Value;
            BatteryHealthText = (health > 1 ? 1 : health).ToString("P");
        }
        else
        {
            BatteryHealthText = "Unknown";
        }

        // Inserts a space between each word in battery status.
        BatteryStatusText = WordStartLetterRegex().Replace(report.Status.ToString(), " $0");
    }
}