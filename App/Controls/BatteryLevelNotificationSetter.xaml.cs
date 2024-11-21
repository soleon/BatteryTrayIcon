using System.Windows;

namespace Percentage.App;

public partial class BatteryLevelNotificationSetter
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked), typeof(bool), typeof(BatteryLevelNotificationSetter), new PropertyMetadata(default(bool)));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(BatteryLevelNotificationSetter), new PropertyMetadata(default(double)));

    public static readonly DependencyProperty StatusNameProperty = DependencyProperty.Register(
        nameof(StatusName), typeof(string), typeof(BatteryLevelNotificationSetter),
        new PropertyMetadata(default(string)));

    public BatteryLevelNotificationSetter()
    {
        InitializeComponent();
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string StatusName
    {
        get => (string)GetValue(StatusNameProperty);
        set => SetValue(StatusNameProperty, value);
    }
}