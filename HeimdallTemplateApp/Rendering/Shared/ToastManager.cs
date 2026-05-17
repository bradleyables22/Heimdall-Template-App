using Heimdall.Bootstrap;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using System.Security.Cryptography;
using static Heimdall.Bootstrap.Bootstrap;

namespace HeimdallTemplateApp.Rendering.Shared
{
	public enum ToastType
	{
		Success,
		Error,
		Warning,
		Info
	}

	public class ToastItem
	{
		public string Header { get; set; } = string.Empty;
		public string Content { get; set; } = string.Empty;
		public ToastType Type { get; set; } = ToastType.Info;
		public int DurationMs { get; set; } = 3000;
	}

	public static class ToastManager
	{
		public const string Id = "toast-manager";
		private const string ToastChannelCookieName = "heimdall.toast-channel";
		private const string ToastChannelItemKey = "__heimdallToastChannelId";
		private const string ToastTopicPrefix = "toasts:user:";

		/// <summary>
		/// This will get or create a cookie-based channel ID for the user and return a unique topic name for that channel. This allows you to send toast notifications to specific users by publishing to their unique topic.
		/// If you have a different user identification strategy, you can modify this method to generate topic names based on your user IDs or session IDs instead. Just ensure that the topic name is unique per user and consistent across requests for the same user.
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public static string GetUserToastTopic(this HttpContext ctx) => $"{ToastTopicPrefix}{ctx.GetOrCreateToastChannelId()}";

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

		private static bool IsValidChannelId(string channelId)
			=> channelId.Length == 32 && channelId.All(Uri.IsHexDigit);

		public static IHtmlContent Render(HttpContext ctx, bool useSSE = true)
			=> FluentHtml.Div(root =>
			{
				root.Class(
					Bootstrap.Toast.Container,
					Bootstrap.Position.Fixed,
					Bootstrap.Position.Top0,
					Bootstrap.Position.End0,
					Bootstrap.Spacing.P(3)
				).Style("z-index:1000")
				.Id(Id)
				.Aria("live", "polite")
				.Aria("atomic", "true");

				if (useSSE)
				{
					//This is a simple example of using Heimdall's SSE features to push toast updates to the client.
					//update the topic name and target selector as needed to fit your application's structure.

					root.Heimdall()
					.SseTopic(ctx.GetUserToastTopic())
					.SseTarget($"#{Id}")
					.SseSwap(HeimdallHtml.Swap.BeforeEnd);
				}

				root.Script(s => s.Src("components/js/toast-manager.js"));
			});

		public static IHtmlContent Create(ToastItem toast)
		{
			var toastClass = toast.GetToastClass();

			return FluentHtml.Div(toastDiv =>
			{
				toastDiv.Class(
					Bootstrap.Toast.ToastBase,
					Bootstrap.Visibility.Fade,
					toastClass
				)
				.Role("alert")
				.Aria("live", "assertive")
				.Aria("atomic", "true")
				.Data("bs-autohide", "true")
				.Data("bs-delay", toast.DurationMs.ToString());

				// Header
				toastDiv.Div(header =>
				{
					header.Class(
						Bootstrap.Toast.Header,
						toastClass,
						Bootstrap.Border.None
					)
					.Tag("strong", strong =>
					{
						strong.Class(Bootstrap.Spacing.MeAuto())
						.Text(toast.Header);
					})
					.Button(close =>
					{
						close.Class(
							Bootstrap.Btn.Close,
							Bootstrap.Spacing.Mb(1),
							Bootstrap.Spacing.Ms(2)
						)
						.Data("bs-dismiss", "toast")
						.Aria("label", "Close");
					});
				});

				// Body
				toastDiv.Div(body =>
				{
					body.Class(Bootstrap.Toast.Body)
					.Text(toast.Content);
				});
			});
		}

		static string GetToastClass(this ToastItem toast)
		{
			return toast.Type switch
			{
				ToastType.Success => Bootstrap.Bg.TextBg(Bootstrap.Color.Success),
				ToastType.Error => Bootstrap.Bg.TextBg(Bootstrap.Color.Danger),
				ToastType.Warning => Bootstrap.Bg.TextBg(Bootstrap.Color.Warning),
				ToastType.Info => Bootstrap.Bg.TextBg(Bootstrap.Color.Info),
				_ => Bootstrap.Bg.TextBg(Bootstrap.Color.Secondary),
			};
		}
	}
}
