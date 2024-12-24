using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Toolkit.Uwp.Notifications;
using Percentage.App.Extensions;
using Percentage.App.Pages;
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
    internal static readonly ISnackbarService SnackBarService = new SnackbarService();

    private readonly Mutex _appMutex;

    public App()
    {
        _appMutex = new Mutex(true, Id, out var isNewInstance);

        if (!isNewInstance)
        {
            Shutdown(1);
            return;
        }

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        DispatcherUnhandledException += (_, e) => HandleException(e.Exception);

        AppDomain.CurrentDomain.UnhandledException += (_, e) => HandleException(e.ExceptionObject);

        TaskScheduler.UnobservedTaskException += (_, e) => HandleException(e.Exception);

        InitializeComponent();

        // User settings migration for backward compatibility.
        MigrateUserSettings();

        // Subscribe to toast notification activations.
        ToastNotificationManagerCompat.OnActivated += OnToastNotificationActivatedAsync;
    }

    internal static MainWindow ActivateMainWindow()
    {
        var window = Current.Windows.OfType<MainWindow>().FirstOrDefault();
        if (window != null)
        {
            window.Activate();
            return window;
        }

        window = new MainWindow();
        window.Show();
        return window;
    }

    internal static Exception GetTrayIconUpdateError()
    {
        return _trayIconUpdateError;
    }

    internal static NotifyIconWindow GetTrayIconWindow()
    {
        return Current.Windows.OfType<NotifyIconWindow>().FirstOrDefault();
    }

    private static void HandleException(object exception)
    {
        var version = VersionExtensions.GetAppVersion();

        if (exception is OutOfMemoryException)
        {
            MessageBox.Show(
                $"Battery Percentage Icon version {version} did not have enough memory to perform some work.\r\n" +
                "Please consider closing some running applications or background services to free up some memory.",
                "Your system memory is running low",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        else
        {
            const string title = "You Found An Error";
            var message =
                $"Battery Percentage Icon version {version} has run into an error. You can help to fix this by:\r\n" +
                "1. Press Ctrl+C on this message\r\n" +
                "2. Report the copied error at https://github.com/soleon/Percentage/issues\r\n\r\n" +
                (exception is Exception exp
                    ? exp.ToString()
                    : $"Error type: {exception.GetType().FullName}\r\n{exception}");
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
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

        Default.Save();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Save user settings when exiting the app.
        _appMutex.Dispose();
        base.OnExit(e);
    }

    private void OnToastNotificationActivatedAsync(ToastNotificationActivatedEventArgsCompat toastArgs)
    {
        var arguments = ToastArguments.Parse(toastArgs.Argument);
        if (!arguments.TryGetActionArgument(out var action))
        {
            // When there's no action from toast notification activation, this is most likely triggered by users
            // clicking the entire notification instead of an individual button.
            // Show the details view in this case.
            Dispatcher.InvokeAsync(() => ActivateMainWindow().NavigateToPage<DetailsPage>());
        }
        
        switch (action)
        {
            case ToastNotificationExtensions.Action.ViewDetails:
                Dispatcher.InvokeAsync(() => ActivateMainWindow().NavigateToPage<DetailsPage>());
                break;
            case ToastNotificationExtensions.Action.DisableBatteryNotification:
                if (!arguments.TryGetNotificationTypeArgument(out var type)) break;
                switch (type)
                {
                    case ToastNotificationExtensions.NotificationType.Critical:
                        Default.BatteryCriticalNotification = false;
                        break;
                    case ToastNotificationExtensions.NotificationType.Low:
                        Default.BatteryLowNotification = false;
                        break;
                    case ToastNotificationExtensions.NotificationType.High:
                        Default.BatteryHighNotification = false;
                        break;
                    case ToastNotificationExtensions.NotificationType.Full:
                        Default.BatteryFullNotification = false;
                        break;
                    case ToastNotificationExtensions.NotificationType.None:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, $"{type} is not supported.");
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, $"{action} is not supported.");
        }
    }
}