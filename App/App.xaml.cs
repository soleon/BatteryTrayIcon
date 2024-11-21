using System;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui;
using static Percentage.App.Properties.Settings;

namespace Percentage.App;

public partial class App
{
    internal static readonly ISnackbarService SnackbarService = new SnackbarService();
    
    public App()
    {
        DispatcherUnhandledException += (_, e) => HandleException(e.Exception);

        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException(e.ExceptionObject);

        TaskScheduler.UnobservedTaskException += (_, e) => HandleException(e.Exception);
        
        InitializeComponent();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Save user settings when exiting the app.
        Default.Save();
        base.OnExit(e);
    }

    private static void HandleException(object exception)
    {
        if (exception is OutOfMemoryException)
            MessageBox.Show("Battery Percentage Icon did not have enough memory to perform some work.\r\n" +
                            "Please consider closing some running applications or background services to free up some memory.",
                "Your system memory is running low",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        else
        {
            const string title = "You Found An Error";
            var message = "Battery Percentage Icon has run into an error. You can help to fix this by:\r\n" +
                          "1. press Ctrl+C on this message\r\n" +
                          "2. paste it in an email\r\n" +
                          "3. send it to soleon@live.com\r\n\r\n" +
                          (exception is Exception exp
                              ? exp.ToString()
                              : $"Error type: {exception.GetType().FullName}\r\n{exception}");
            try
            {
                new Wpf.Ui.Controls.MessageBox
                {
                    Title = title,
                    Content = message
                }.ShowDialogAsync().GetAwaiter().GetResult();
            }
            catch
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}