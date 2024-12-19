using System;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel;

namespace Percentage.App.Extensions;

internal static class ExternalProcessExtensions
{
    public static void OpenDonationLocation()
    {
        StartShellExecutedProgress("https://www.paypal.com/donate/?hosted_button_id=EFS3E8WPF8SJL");
    }

    internal static void OpenFeedbackLocation()
    {
        StartShellExecutedProgress("https://github.com/soleon/Percentage/issues");
    }

    internal static void OpenSourceCodeLocation()
    {
        StartShellExecutedProgress("https://github.com/soleon/Percentage");
    }

    internal static void ShowRatingView()
    {
        StartShellExecutedProgress("ms-windows-store://review/?ProductId=9PCKT2B7DZMW");
    }

    internal static void OpenPowerSettings()
    {
        StartShellExecutedProgress("ms-settings:powersleep");
    }

    private static void StartShellExecutedProgress(string fileName)
    {
        Process.Start(new ProcessStartInfo(fileName)
        {
            UseShellExecute = true
        });
    }
}