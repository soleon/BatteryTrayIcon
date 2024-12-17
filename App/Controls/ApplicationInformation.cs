using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Percentage.App.Controls;

public sealed class ApplicationInformation : KeyValueItemsControl
{
    public ApplicationInformation()
    {
        ItemsSource = new Dictionary<string, object>
        {
            { "App version", Helper.GetAppVersion() },
            { "Runtime version", RuntimeInformation.FrameworkDescription },
            { "Runtime architecture", RuntimeInformation.RuntimeIdentifier }
        };
    }
}