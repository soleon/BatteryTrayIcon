using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FontFamily = System.Drawing.FontFamily;

namespace Percentage.Wpf.Converters
{
    internal class NameToFontFamilyConverter : IValueConverter
    {
        public static NameToFontFamilyConverter Instance { get; } = new NameToFontFamilyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FontFamilyConverter().ConvertFromString(((FontFamily) value)
                .Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}