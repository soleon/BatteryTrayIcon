using System;
using System.Diagnostics;

namespace Percentage.App;

internal static class Helper
{
    public static string GetReadableSize(long bytes)
    {
        string[] sizeSuffixes = ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB"];

        switch (bytes)
        {
            case < 0:
            case 0:
                return "0 Bytes";
        }

        var order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizeSuffixes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizeSuffixes[order]}";
    }

    internal static string GetReadableTimeSpan(TimeSpan timeSpan)
    {
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes;
        return timeSpan.TotalSeconds < 60
            ? "less than 1 minute"
            : (hours > 0
                  ? hours > 1 ? hours + " hours " : "1 hour "
                  : null) +
              (minutes > 0
                  ? minutes > 1 ? minutes + " minutes" : "1 minute"
                  : null);
    }

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

    private static void StartShellExecutedProgress(string fileName)
    {
        Process.Start(new ProcessStartInfo(fileName)
        {
            UseShellExecute = true
        });
    }
}