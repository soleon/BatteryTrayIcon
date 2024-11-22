using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;

namespace Percentage.App.Controls;

public sealed class SystemInformation : KeyValueItems
{
    public SystemInformation()
    {
        ItemsSource = new Dictionary<string, object> { { "Loading", "please wait..." } };
        Task.Run(() =>
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

            var cpuInfo = string.Join(Environment.NewLine,
                searcher.Get().OfType<ManagementBaseObject>().Select(x => x["Name"]));

            var computerInfo = new ComputerInfo();
            var totalMemory = computerInfo.TotalPhysicalMemory;

            return Dispatcher.InvokeAsync(() => ItemsSource = new Dictionary<string, object>
            {
                { "Windows version", RuntimeInformation.OSDescription },
                { "Architecture", RuntimeInformation.OSArchitecture },
                { "Processor", cpuInfo },
                { "Memory", Helper.GetReadableSize((long)totalMemory) }
            });
        });
    }
}