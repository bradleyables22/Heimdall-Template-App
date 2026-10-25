using System.Security.Cryptography;

namespace HeimdallTemplateApp.Infrastructure;

public static class ToastChannelExtensions
{
	private const string ToastChannelCookieName = "heimdall.toast-channel";
	private const string ToastChannelItemKey = "__heimdallToastChannelId";
	private const string ToastTopicPrefix = "toasts:user:";

	public static string GetUserToastTopic(this HttpContext ctx) =>
		$"{ToastTopicPrefix}{ctx.GetOrCreateToastChannelId()}";

	private static string GetOrCreateToastChannelId(this HttpContext ctx)
	{
		if (ctx.Items.TryGetValue(ToastChannelItemKey, out var cachedChannelId) &&
			cachedChannelId is string channelId)
			return channelId;

		if (ctx.Request.Cookies.TryGetValue(ToastChannelCookieName, out var existingChannelId) &&
			IsValidChannelId(existingChannelId))
		{
			ctx.Items[ToastChannelItemKey] = existingChannelId;
			return existingChannelId;
		}

		var newChannelId = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
		ctx.Items[ToastChannelItemKey] = newChannelId;

		if (!ctx.Response.HasStarted)
		{
			ctx.Response.Cookies.Append(
				ToastChannelCookieName,
				newChannelId,
				new CookieOptions
				{
					HttpOnly = true,
					IsEssential = true,
					Path = "/",
					SameSite = SameSiteMode.Lax,
					Secure = ctx.Request.IsHttps
				});
		}

		return newChannelId;
	}

	private static bool IsValidChannelId(string channelId) =>
		channelId.Length == 32 && channelId.All(Uri.IsHexDigit);
}
