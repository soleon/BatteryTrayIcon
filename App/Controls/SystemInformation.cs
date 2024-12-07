using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic.Devices;

namespace Percentage.App.Controls;

public sealed class SystemInformation : KeyValueItemsControl
{
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
        nameof(IsLoading), typeof(bool), typeof(SystemInformation), new PropertyMetadata(default(bool)));

    public SystemInformation()
    {
        IsLoading = true;
        Task.Run(() =>
        {
            string cpuInfo;
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

                cpuInfo = string.Join(Environment.NewLine,
                    searcher.Get().OfType<ManagementBaseObject>().Select(x => x["Name"]));
            }
            catch
            {
                cpuInfo = "Unknown";
            }

            return Dispatcher.InvokeAsync(() =>
            {
                ItemsSource = new Dictionary<string, object>
                {
                    { "Windows version", RuntimeInformation.OSDescription },
                    { "Processor", cpuInfo },
                    { "Memory", Helper.GetReadableSize((long)new ComputerInfo().TotalPhysicalMemory) }
                };
                IsLoading = false;
            });
        });
    }

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
}