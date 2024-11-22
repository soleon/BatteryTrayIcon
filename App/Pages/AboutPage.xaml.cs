using System.Windows;

namespace Percentage.App.Pages;

public partial class AboutPage
{
    public AboutPage()
    {
        InitializeComponent();
    }

    private void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.SendFeedBack();
    }

    private void OnRatingButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.ShowRatingView();
    }

    private void OnSourceCodeButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.OpenSourceCodeLocation();
    }
}