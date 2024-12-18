using Microsoft.Toolkit.Uwp.Notifications;

namespace Percentage.App.Extensions;

internal static class ToastNotificationExtensions
{
    private const string ActionArgumentKey = "action";

    internal static Action GetActionArgument(this ToastArguments arguments)
    {
        return arguments.GetEnum<Action>(ActionArgumentKey);
    }

    internal static void ShowToastNotification(string header, string body)
    {
        new ToastContentBuilder()
            .AddText(header)
            .AddText(body)
            .AddButton(new ToastButton().SetContent("See more details")
                .AddArgument(ActionArgumentKey, Action.ViewDetails))
            .AddButton(new ToastButtonDismiss())
            .Show();
    }

    internal enum Action
    {
        ViewDetails = 0
    }
}