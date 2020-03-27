using System.Reflection;
using System.Windows.Media;
using Microsoft.Win32;

namespace Wpf
{
    internal static class Helper
    {
        private const string WindowsCurrentVersionSubKey = @"Software\Microsoft\Windows\CurrentVersion\";
        private static readonly string ProductName;
        private static readonly string ExecutablePath;

        static Helper()
        {
            var assembly = typeof(Helper).Assembly;
            ProductName = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
            ExecutablePath = assembly.Location;
        }

        /// <summary>
        ///     Opens the auto start registry key.
        /// </summary>
        /// <returns></returns>
        private static RegistryKey OpenAutoStartKey()
        {
            return Registry.CurrentUser.OpenSubKey(WindowsCurrentVersionSubKey + "Run", true);
        }

        /// <summary>
        ///     Check if the auto start registry key presents.
        /// </summary>
        internal static bool IsAutoStart()
        {
            return OpenAutoStartKey()?.GetValue(ProductName, null) != null;
        }

        /// <summary>
        ///     Adds auto start registry key.
        /// </summary>
        internal static void EnableAutoStart()
        {
            OpenAutoStartKey().SetValue(ProductName, ExecutablePath);
        }

        /// <summary>
        ///     Removes auto start registry key.
        /// </summary>
        internal static void DisableAutoRun()
        {
            OpenAutoStartKey().DeleteValue(ProductName, false);
        }

        /// <summary>
        ///     Determine if the Windows 10 light theme is in use.
        ///     False if dark theme is in use or registry key doesn't exist
        ///     for versions of Windows that don't support light theme.
        /// </summary>
        internal static bool IsUsingLightTheme()
        {
            var key = Registry.CurrentUser.OpenSubKey(WindowsCurrentVersionSubKey + @"Themes\Personalize\", false);
            return key != null && 1.Equals(key.GetValue("SystemUsesLightTheme", null));
        }

        /// <summary>
        ///     Converts a <see cref="System.Drawing.Color" /> object to a <see cref="System.Windows.Media.Color" /> object.
        /// </summary>
        internal static Color ToMediaColor(this System.Drawing.Color color)
        {
            return Color.FromRgb(color.R, color.G, color.B);
        }
    }
}