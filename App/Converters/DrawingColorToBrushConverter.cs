using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Percentage.App.Converters;

internal class DrawingColorToBrushConverter : IValueConverter
{
    public static DrawingColorToBrushConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var drawingColor = (Color)value;
        return new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R,
            drawingColor.G, drawingColor.B));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}