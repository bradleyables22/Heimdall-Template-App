using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Rendering.Shared;
using Heimdall.Bootstrap;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Pages
{
	public class OobPage
	{
		private static readonly HeimdallHtml.ActionId ActionToastSse = "OobPage.ToastSSEHello";
		private static readonly HeimdallHtml.ActionId ActionToastOob = "OobPage.ToastHello";

		private static IHtmlContent BtnSse => FluentHtml.Button(b =>
		{
			b.Type("button")
			.Class(Bootstrap.Btn.Primary, Bootstrap.Spacing.Me(2))
			.Add(
				HeimdallHtml.OnClick(ActionToastSse),
				HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
				HeimdallHtml.PayloadEmptyObject()
			)
			.Text("Send toast via SSE");
		});

		private static IHtmlContent BtnOob => FluentHtml.Button(b =>
		{
			b.Type("button")
			.Class(Bootstrap.Btn.OutlinePrimary)
			.Add(
				HeimdallHtml.OnClick(ActionToastOob),
				HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
				HeimdallHtml.PayloadEmptyObject()
			)
			.Text("Send toast via OOB Invocation");
		});

		private static IHtmlContent Explainer => FluentHtml.Div(d =>
		{
			d.Class(
				Bootstrap.Alert.Variant(Bootstrap.Color.Light),
				Bootstrap.Spacing.Mt(3),
				Bootstrap.Text.Align(Bootstrap.TextAlignKind.Start)
			)
			.H5(h => h.Text("Where the toast manager comes from"))
			.P(p =>
			{
				p.Text("This page does not render the toast container. ")
				.Strong(s => s.Text("The layout is responsible for rendering "))
				.Code(c => c.Text("#toast-manager"))
				.Text(" (and, if enabled, subscribing it to the ")
				.Code(c => c.Text("toasts"))
				.Text(" topic via SSE).");
			})
			.Hr(_ => { })
			.H6(h => h.Text("Two methodologies"))
			.Ul(ul =>
			{
				ul.Li(li =>
				{
					li.Strong(s => s.Text("SSE (Bifrost): "))
					.Text("The click calls a server action that publishes toast HTML to topic ")
					.Code(c => c.Text("toasts"))
					.Text(". Any connected page subscribed to that topic receives the toast.");
				})
				.Li(li =>
				{
					li.Strong(s => s.Text("OOB Invocation: "))
					.Text("The click calls a server action that returns an ")
					.Code(c => c.Text("<invocation>"))
					.Text(" targeting ")
					.Code(c => c.Text("#toast-manager"))
				    .Text(". Heimdall.js applies it client-side without rendering the invocation element.");
				});
			});
		});

		public static IHtmlContent Render()
		{
			var card = FluentHtml.Div(d =>
			{
				d.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg)
				.Div(body =>
				{
					body.Class(
						Bootstrap.Card.Body,
						Bootstrap.Text.Align(Bootstrap.TextAlignKind.Center)
					)
					.H1(h => h.Text("Send a toast!"))
					.P(p => p.Text("Use either approach below. Both ultimately insert a toast into the layout’s #toast-manager."))
					.Div(btnRow =>
					{
						btnRow.Class(Bootstrap.Spacing.Mt(3))
						.Add(BtnSse, BtnOob);
					})
					.Add(Explainer);
				});
			});

			return FluentHtml.Div(frame =>
			{
				frame.Class(
					Bootstrap.Spacing.M(3, Bootstrap.Side.Top),
					Bootstrap.Layout.ContainerFluid
				)
				.Div(row =>
				{
					row.Class(Bootstrap.Layout.Row)
					.Div(col => col.Class(Bootstrap.Layout.Col))
					.Div(center =>
					{
						center.Class(Bootstrap.Layout.ColSpan(6, Bootstrap.Breakpoint.Lg))
						.Add(card);
					})
					.Div(col => col.Class(Bootstrap.Layout.Col));
				});
			});
		}

		[ContentInvocation]
		public static async Task<IHtmlContent> ToastSSEHello(Bifrost bifrost,HttpContext ctx)
		{
			var toast = new ToastItem
			{
				Header = "Hello from SSE!",
				Content = "This toast was published to the 'toasts' topic and delivered over EventSource.",
				Type = ToastType.Success,
				DurationMs = 1800
			};

			var html = ToastManager.Create(toast);

			//This is a simple example of using Heimdall's SSE features to push toast updates to the client.
			//update the topic name and target selector as needed to fit your application's structure.
			await bifrost.PublishAsync($"toasts:user:{ctx.Connection.Id}", html, TimeSpan.FromSeconds(10));

			return HtmlString.Empty;
		}

		[ContentInvocation]
		public static Task<IHtmlContent> ToastHello()
		{
			var toast = new ToastItem
			{
				Header = "Hello from OOB!",
				Content = "This toast came back as an <invocation> targeting #toast-manager.",
				Type = ToastType.Success,
				DurationMs = 1800
			};

			var html = ToastManager.Create(toast);

			IHtmlContent result = FluentHtml.Fragment(f =>
			{
				f.Add(
					HeimdallHtml.Invocation(
						targetSelector: "#toast-manager",
						swap: HeimdallHtml.Swap.AfterBegin,
						payload: html,
						wrapInTemplate: false
					)
				);
			});

			return Task.FromResult(result);
		}
	}
}
