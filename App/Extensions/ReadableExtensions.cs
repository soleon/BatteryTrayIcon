using System;

namespace Percentage.App.Extensions;

internal static class ReadableExtensions
{
    internal static string GetReadableSize(this long bytes)
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
}