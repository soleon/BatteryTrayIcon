using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FontFamily = System.Drawing.FontFamily;

namespace Percentage.Wpf.Converters
{
    internal class DrawingFontToMediaFontConverter : IValueConverter
    {
        public static DrawingFontToMediaFontConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FontFamilyConverter().ConvertFromString(((FontFamily) value).Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}