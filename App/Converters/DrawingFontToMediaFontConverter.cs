using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FontFamily = System.Drawing.FontFamily;

namespace Percentage.App.Converters;

internal class DrawingFontToMediaFontConverter : IValueConverter
{
    private static readonly FontFamilyConverter FontFamilyConverter = new();
    public static DrawingFontToMediaFontConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return FontFamilyConverter.ConvertFromString(((FontFamily)value).Name);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}