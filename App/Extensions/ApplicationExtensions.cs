using System.Linq;
using System.Windows;

namespace Percentage.App.Extensions;

internal static class ApplicationExtensions
{
    internal static MainWindow ActivateMainWindow(this Application app)
    {
        var window = app.Windows.OfType<MainWindow>().FirstOrDefault();
        if (window != null)
        {
            window.Activate();
            return window;
        }

        window = new MainWindow();
        window.Show();
        return window;
    }
    
    internal static NotifyIconWindow GetNotifyIconWindow(this Application app)
    {
        return app.Windows.OfType<NotifyIconWindow>().FirstOrDefault();
    }
}