using System;
using System.Diagnostics;

namespace Percentage.App;

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
        Process.Start(new ProcessStartInfo("mailto:soleon@live.com?subject=Battery Percentage Icon Feedback")
        {
            UseShellExecute = true
        });
    }

    internal static void ShowRatingView()
    {
        Process.Start(new ProcessStartInfo("ms-windows-store://review/?ProductId=9PCKT2B7DZMW")
        {
            UseShellExecute = true
        });
    }
}