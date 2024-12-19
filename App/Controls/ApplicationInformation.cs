using System.Collections.Generic;
using System.Runtime.InteropServices;
using Percentage.App.Extensions;

namespace Percentage.App.Controls;

public sealed class ApplicationInformation : KeyValueItemsControl
{
    public ApplicationInformation()
    {
        ItemsSource = new Dictionary<string, object>
        {
            { "App version", VersionExtensions.GetAppVersion() },
            { "Runtime version", RuntimeInformation.FrameworkDescription },
            { "Runtime architecture", RuntimeInformation.RuntimeIdentifier }
        };
    }
}