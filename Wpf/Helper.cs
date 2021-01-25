using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Percentage.Wpf
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

        public static void SendFeedBack()
        {
            Process.Start(
                "mailto:soleon@live.com?subject=Battery Percentage Icon Feedback&body=Thank you for sending in your feedback.%0D%0ALet me know what's in your mind.%0D%0A%0D%0ACheers,%0D%0ALong");
        }
    }
}