using System.Reflection;
using Windows.ApplicationModel;

namespace Percentage.App.Extensions;

public static class VersionExtensions
{
    internal static string GetAppVersion()
    {
        string version;
        try
        {
            version = Package.Current.Id.Version.ToVersionString();
        }
        catch
        {
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        }

        return version;
    }
}