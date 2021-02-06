using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using Percentage.Wpf.Properties;

namespace Percentage.Wpf.Converters
{
    internal class FontNameSettingFontConverter : IValueConverter
    {
        public static FontNameSettingFontConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Font) value).FontFamily.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            using var existingFont = Settings.Default.TrayIconFont;
            return new Font(new FontFamily((string) value), existingFont.Size);
        }
    }
}