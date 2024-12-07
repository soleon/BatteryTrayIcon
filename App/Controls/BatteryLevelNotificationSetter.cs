using System.Windows;
using System.Windows.Controls;

namespace Percentage.App.Controls;

public class BatteryLevelNotificationSetter : Control
{
    public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
        nameof(IsChecked), typeof(bool), typeof(BatteryLevelNotificationSetter));

    public static readonly DependencyProperty StatusNameProperty = DependencyProperty.Register(
        nameof(StatusName), typeof(string), typeof(BatteryLevelNotificationSetter));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(double), typeof(BatteryLevelNotificationSetter));

    static BatteryLevelNotificationSetter()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(BatteryLevelNotificationSetter),
            new FrameworkPropertyMetadata(typeof(BatteryLevelNotificationSetter)));
    }

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string StatusName
    {
        get => (string)GetValue(StatusNameProperty);
        set => SetValue(StatusNameProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}