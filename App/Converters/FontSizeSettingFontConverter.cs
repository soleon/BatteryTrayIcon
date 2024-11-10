using System.Globalization;
using System.Windows.Data;

namespace Percentage.App.Converters;

internal class FontSizeSettingFontConverter : IValueConverter
{
    public static FontSizeSettingFontConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ((Font)value).Size;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        using var existingFont = Settings.Default.TrayIconFont;
        return new Font(existingFont.FontFamily, (float)value);
    }
}