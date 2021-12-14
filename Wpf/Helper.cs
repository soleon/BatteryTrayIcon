using System;
using System.Diagnostics;

namespace Percentage.Wpf;

internal static class Helper
{
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

    internal static void SendFeedBack()
    {
        Process.Start(
            "mailto:soleon@live.com?subject=Battery Percentage Icon Feedback&body=Thank you for sending in your feedback.%0D%0ALet me know what's in your mind.%0D%0A%0D%0ACheers,%0D%0ALong");
    }

    internal static void ShowRatingView()
    {
        Process.Start("ms-windows-store://review/?ProductId=9PCKT2B7DZMW");
    }
}