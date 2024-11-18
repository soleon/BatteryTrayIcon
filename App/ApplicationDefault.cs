using System.Windows.Media;

namespace Percentage.App;

internal static class ApplicationDefault
{
    internal const string BatteryCriticalColour = "#E81123";
    internal const string BatteryLowColour = "#CA5010";
    internal const string BatteryChargingColour = "#10893E";
    internal const string BatteryNormalColour = null;
    internal static readonly FontFamily TrayIconFontFamily = new("Microsoft Sans Serif");
    internal const int BatteryCriticalNotificationValue = 10;
    internal const int BatteryLowNotificationValue = 20;
    internal const int BatteryHighNotificationValue = 80;
    internal const int RefreshSeconds = 60;
    internal const bool BatteryFullNotification = true;
    internal const bool BatteryLowNotification = true;
    internal const bool BatteryHighNotification = true;
    internal const bool BatteryCriticalNotification = true;
    internal const bool HideAtStartup = false;
    internal const bool TrayIconFontBold = false;
    internal const bool TrayIconFontUnderline = false;
}