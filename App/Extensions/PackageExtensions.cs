using Windows.ApplicationModel;

namespace Percentage.App.Extensions;

internal static class PackageExtensions
{
    internal static string ToVersionString(this PackageVersion version)
    {
        return string.Join('.', version.Major, version.Minor, version.Build);
    }
}