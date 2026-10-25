using System.Security.Cryptography;

namespace HeimdallTemplateApp.Infrastructure;

public static class ToastChannelExtensions
{
    private const string ToastChannelCookieName = "heimdall.toast-channel";
    private const string ToastChannelItemKey = "__heimdallToastChannelId";
    private const string ToastTopicPrefix = "toasts:user:";

    public static string GetUserToastTopic(this HttpContext context)
        => $"{ToastTopicPrefix}{context.GetOrCreateToastChannelId()}";

    private static string GetOrCreateToastChannelId(this HttpContext context)
    {
        if (context.Items.TryGetValue(ToastChannelItemKey, out var cached)
            && cached is string cachedId)
        {
            return cachedId;
        }

        if (context.Request.Cookies.TryGetValue(ToastChannelCookieName, out var existing)
            && IsValidChannelId(existing))
        {
            context.Items[ToastChannelItemKey] = existing;
            return existing;
        }

        var channelId = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
        context.Items[ToastChannelItemKey] = channelId;

        if (!context.Response.HasStarted)
        {
            context.Response.Cookies.Append(
                ToastChannelCookieName,
                channelId,
                new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = true,
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Secure = context.Request.IsHttps
                });
        }

        return channelId;
    }

    private static bool IsValidChannelId(string value)
        => value.Length == 32 && value.All(Uri.IsHexDigit);
}
