using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Markup;
using static Percentage.App.Properties.Settings;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

namespace Percentage.App;

public partial class App
{
    internal const string DefaultBatteryChargingColour = "#FF10893E";
    internal const string DefaultBatteryCriticalColour = "#FFE81123";
    internal const bool DefaultBatteryCriticalNotification = true;
    internal const int DefaultBatteryCriticalNotificationValue = 10;
    internal const bool DefaultBatteryFullNotification = true;
    internal const bool DefaultBatteryHighNotification = true;
    internal const int DefaultBatteryHighNotificationValue = 80;
    internal const string DefaultBatteryLowColour = "#FFCA5010";
    internal const bool DefaultBatteryLowNotification = true;
    internal const int DefaultBatteryLowNotificationValue = 20;
    internal const bool DefaultHideAtStartup = false;
    internal const bool DefaultIsAutoBatteryChargingColour = false;
    internal const bool DefaultIsAutoBatteryCriticalColour = false;
    internal const bool DefaultIsAutoBatteryLowColour = false;
    internal const bool DefaultIsAutoBatteryNormalColour = true;
    internal const int DefaultRefreshSeconds = 60;
    internal const bool DefaultTrayIconFontBold = false;
    internal const bool DefaultTrayIconFontUnderline = false;
    internal const string Id = "f05f920a-c997-4817-84bd-c54d87e40625";
    private static Exception _trayIconUpdateError;
    internal static readonly FontFamily DefaultTrayIconFontFamily = new("Microsoft Sans Serif");
    internal static readonly ISnackbarService SnackbarService = new SnackbarService();

    private readonly Mutex _appMutex;

    public App()
    {
        _appMutex = new Mutex(true, Id, out var isNewInstance);

        if (!isNewInstance) Shutdown(1);

        DispatcherUnhandledException += (_, e) => HandleException(e.Exception);

        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException(e.ExceptionObject);

        TaskScheduler.UnobservedTaskException += (_, e) => HandleException(e.Exception);

        InitializeComponent();

        // User settings migration for backward compatibility.
        MigrateUserSettings();
    }

    internal static Exception GetTrayIconUpdateError()
    {
        return _trayIconUpdateError;
    }

    private static void HandleException(object exception)
    {
        if (exception is OutOfMemoryException)
        {
            MessageBox.Show("Battery Percentage Icon did not have enough memory to perform some work.\r\n" +
                            "Please consider closing some running applications or background services to free up some memory.",
                "Your system memory is running low",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
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

    internal static void SetTrayIconUpdateError(Exception e)
    {
        _trayIconUpdateError = e;
        TrayIconUpdateErrorSet?.Invoke(e);
    }

    internal static event Action<Exception> TrayIconUpdateErrorSet;

    private void MigrateUserSettings()
    {
        if (Default.RefreshSeconds < 5) Default.RefreshSeconds = 5;

        Default.BatteryNormalColour ??=
            ((Brush)FindResource(nameof(ThemeResource.TextFillColorPrimaryBrush)))!.ToString();

        if (Default.BatteryLowColour is { Length: 7 } lowColourHexValue)
            Default.BatteryLowColour = lowColourHexValue.Insert(1, "FF");

        if (Default.BatteryCriticalColour is { Length: 7 } criticalColourHexValue)
            Default.BatteryCriticalColour = criticalColourHexValue.Insert(1, "FF");

        if (Default.BatteryChargingColour is { Length: 7 } chargingColourHexValue)
            Default.BatteryChargingColour = chargingColourHexValue.Insert(1, "FF");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Save user settings when exiting the app.
        _appMutex.Dispose();
        base.OnExit(e);
    }
}