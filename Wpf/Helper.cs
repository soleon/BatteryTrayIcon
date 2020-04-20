using System.Text.RegularExpressions;

namespace Wpf
{
    internal static class Helper
    {
        internal static string CamelCaseSplit(this string value)
        {
            return Regex.Replace(value, @"\B[A-Z]", " $0");
        }
    }
}