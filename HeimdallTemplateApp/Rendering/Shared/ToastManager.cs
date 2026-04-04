using HeimdallTemplateApp.Rendering.Utilities;
using Microsoft.AspNetCore.Html;
using Heimdall.Server.Rendering;

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
		public static IHtmlContent Render(HttpContext ctx, bool useSSE = true)
			=> FluentHtml.Div(root =>
			{
				root.Class(
					Bootstrap.Toast.Container,
					Bootstrap.Position.Fixed,
					Bootstrap.Position.Top0,
					Bootstrap.Position.End0,
					Bootstrap.Spacing.P(3)
				);

				root.Style("z-index:1000");
				root.Id("toast-manager");
				root.Aria("live", "polite");
				root.Aria("atomic", "true");

				if (useSSE)
				{
					//This is a simple example of using Heimdall's SSE features to push toast updates to the client.
					//update the topic name and target selector as needed to fit your application's structure.

					root.Heimdall()
					.SseTopic($"toasts:user:{ctx.Connection.Id}")
					.SseTarget("#toast-manager")
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
				);

				toastDiv.Role("alert");
				toastDiv.Aria("live", "assertive");
				toastDiv.Aria("atomic", "true");
				toastDiv.Data("bs-autohide", "true");
				toastDiv.Data("bs-delay", toast.DurationMs.ToString());

				// Header
				toastDiv.Div(header =>
				{
					header.Class(
						Bootstrap.Toast.Header,
						toastClass,
						Bootstrap.Border.None
					);

					header.Tag("strong", strong =>
					{
						strong.Class(Bootstrap.Spacing.MeAuto());
						strong.Text(toast.Header);
					});

					header.Button(close =>
					{
						close.Class(
							Bootstrap.Btn.Close,
							Bootstrap.Spacing.Mb(1),
							Bootstrap.Spacing.Ms(2)
						);
						close.Data("bs-dismiss", "toast");
						close.Aria("label", "Close");
					});
				});

				// Body
				toastDiv.Div(body =>
				{
					body.Class(Bootstrap.Toast.Body);
					body.Text(toast.Content);
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
