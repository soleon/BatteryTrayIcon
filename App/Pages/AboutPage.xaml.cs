using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Percentage.App.Pages;

public sealed partial class AboutPage : INotifyPropertyChanged
{
    public AboutPage()
    {
        InitializeComponent();

        Loaded += (_, _) => App.TrayIconUpdateErrorSet += OnTrayIconUpdateErrorSet;

        Unloaded += (_, _) => App.TrayIconUpdateErrorSet -= OnTrayIconUpdateErrorSet;
    }

    public Exception TrayIconUpdateError => App.GetTrayIconUpdateError();

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnDonationButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.OpenDonationLocation();
    }

    private void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.OpenFeedbackLocation();
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnRatingButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.ShowRatingView();
    }

    private void OnSourceCodeButtonClick(object sender, RoutedEventArgs e)
    {
        Helper.OpenSourceCodeLocation();
    }

    private void OnTrayIconUpdateErrorSet(Exception obj)
    {
        OnPropertyChanged(nameof(TrayIconUpdateError));
    }
}