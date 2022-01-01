using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Percentage.Ui.Net;

public class MetroWindow : Window
{
    static MetroWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow),
            new FrameworkPropertyMetadata(typeof(MetroWindow)));
    }

    public MetroWindow()
    {
        Loaded += (_, _) =>
        {
            if (Template.FindName("CloseButton", this) is Button button)
            {
                button.Click += (_, _) => Close();
            }
        };
        MouseDown += (_, e) =>
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        };
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        };
    }
}