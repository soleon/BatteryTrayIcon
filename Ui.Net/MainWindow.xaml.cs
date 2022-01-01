using System;
using System.Windows;
using Microsoft.Toolkit.Wpf.UI.XamlHost;

namespace Percentage.Ui.Net
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string WPFMessage => "Binding from WPF to UWP XAML";

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            // Hook up x:Bind source.
            var windowsXamlHost = sender as WindowsXamlHost;
            var userControl = windowsXamlHost.GetUwpInternalObject() as Ui.Uwp.SettingsView;

            if (userControl != null)
            {
                userControl.XamlIslandMessage = WPFMessage;
            }
        }
    }
}