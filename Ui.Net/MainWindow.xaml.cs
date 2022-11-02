using Wpf.Ui.Appearance;

namespace Ui.Net;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => Watcher.Watch(this);
        NotifyIcon.Register();
    }
}