using Heimdall.Bootstrap;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
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
		public static string GetUserToastTopic(this HttpContext ctx) => $"toasts:user:{ctx.Connection.Id}";

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
					.SseTopic($"{GetUserToastTopic(ctx)}")
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
