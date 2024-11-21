using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Percentage.App.Extensions;

namespace Percentage.App.Controls;

public class ApplicationInformation : KeyValueItems
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

        var informationItems = new Dictionary<string, object>
        {
            { "App version", version },
            { "Runtime version", RuntimeInformation.FrameworkDescription },
            { "Runtime architecture", RuntimeInformation.RuntimeIdentifier }
        };

        var process = Process.GetCurrentProcess();
        informationItems.Add("App start time", process.StartTime);
        informationItems.Add("App runtime", Helper.GetReadableTimeSpan(process.TotalProcessorTime));

        ItemsSource = informationItems;
    }
}