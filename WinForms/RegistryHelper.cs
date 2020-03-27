using System.Windows.Forms;
using Microsoft.Win32;

namespace Percentage
{
    internal static class RegistryHelper
    {
        private const string WindowsCurrentVersionSubKey = @"Software\Microsoft\Windows\CurrentVersion\";

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
            return OpenAutoStartKey()?.GetValue(Application.ProductName, null) != null;
        }

        /// <summary>
        ///     Adds auto start registry key.
        /// </summary>
        internal static void EnableAutoStart()
        {
            OpenAutoStartKey().SetValue(Application.ProductName, Application.ExecutablePath);
        }

        /// <summary>
        ///     Removes auto start registry key.
        /// </summary>
        internal static void DisableAutoRun()
        {
            OpenAutoStartKey().DeleteValue(Application.ProductName, false);
        }

        /// <summary>
        ///     Determine if the Windows 10 light theme is in use.
        ///     False if dark theme is in use or registry key doesn't exist
        ///     for versions of Windows that don't support light theme.
        /// </summary>
        internal static bool IsUsingLightTheme()
        {
            return 1.Equals(
                Registry.CurrentUser.GetValue(WindowsCurrentVersionSubKey + @"Themes\Personalize\SystemUsesLightTheme",
                    null));
        }
    }
}