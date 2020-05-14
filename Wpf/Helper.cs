using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Wpf
{
    internal static class Helper
    {
        internal static string CamelCaseSplit(this string value)
        {
            return Regex.Replace(value, @"\B[A-Z]", " $0");
        }

        internal static void ShowRatingView()
        {
            Process.Start("ms-windows-store://review/?ProductId=9PCKT2B7DZMW");
        }
    }
}