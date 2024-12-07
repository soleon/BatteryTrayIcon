using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Percentage.App.Extensions;

namespace Percentage.App.Controls;

public sealed class ApplicationInformation : KeyValueItemsControl
{
    public ApplicationInformation()
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

        ItemsSource = new Dictionary<string, object>
        {
            { "App version", version },
            { "Runtime version", RuntimeInformation.FrameworkDescription },
            { "Runtime architecture", RuntimeInformation.RuntimeIdentifier }
        };
    }
}