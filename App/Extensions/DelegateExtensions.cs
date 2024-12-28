using System;

namespace Percentage.App.Extensions;

internal static class DelegateExtensions
{
    private const int DefaultMaxRetryCount = 5;

    internal static void RetryOnException<T>(this Action action, Action<T> onRetryFailed = null,
        int maxRetryCount = DefaultMaxRetryCount) where T : Exception
    {
        var lastRetry = maxRetryCount - 1;
        for (var i = 0; i < maxRetryCount; i++)
            try
            {
                action();
                return;
            }
            catch (T e)
            {
                if (i == lastRetry) onRetryFailed?.Invoke(e);
            }
    }
}