using System.Windows;

namespace Percentage.App.Pages;

public partial class DetailsPage
{
    public DetailsPage()
    {
        InitializeComponent();
    }

    private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        BatteryInformation.RequestUpdate();
        App.GetTrayIconWindow().RequestBatteryStatusUpdate();
    }
}