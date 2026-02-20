using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Heimdall;
using HeimdallTemplateApp.Rendering.Shared;
using HeimdallTemplateApp.Utilities;

namespace HeimdallTemplateApp.Rendering.Pages
{
	public class OobPage
	{
		private static readonly HeimdallHtml.ActionId ActionToastSse = "OobPage.ToastSSEHello";
		private static readonly HeimdallHtml.ActionId ActionToastOob = "OobPage.ToastHello";

		private static IHtmlContent BtnSse => FluentHtml.Button(b =>
		{
			b.Type("button");
			b.Class(Bootstrap.Btn.Primary, Bootstrap.Spacing.Me(2));
			b.Add(
				HeimdallHtml.OnClick(ActionToastSse),
				HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
				HeimdallHtml.PayloadEmptyObject()
			);
			b.Text("Send toast via SSE");
		});

		private static IHtmlContent BtnOob => FluentHtml.Button(b =>
		{
			b.Type("button");
			b.Class(Bootstrap.Btn.OutlinePrimary);
			b.Add(
				HeimdallHtml.OnClick(ActionToastOob),
				HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
				HeimdallHtml.PayloadEmptyObject()
			);
			b.Text("Send toast via OOB Invocation");
		});

		private static IHtmlContent Explainer => FluentHtml.Div(d =>
		{
			d.Class(
				Bootstrap.Alert.Variant(Bootstrap.Color.Light),
				Bootstrap.Spacing.Mt(3),
				Bootstrap.Text.Align(Bootstrap.TextAlignKind.Start)
			);

			d.H5(h => h.Text("Where the toast manager comes from"));

			d.P(p =>
			{
				p.Text("This page does not render the toast container. ");
				p.Strong(s => s.Text("The layout is responsible for rendering "));
				p.Code(c => c.Text("#toast-manager"));
				p.Text(" (and, if enabled, subscribing it to the ");
				p.Code(c => c.Text("toasts"));
				p.Text(" topic via SSE).");
			});

			d.Hr(_ => { });

			d.H6(h => h.Text("Two methodologies"));

			d.Ul(ul =>
			{
				ul.Li(li =>
				{
					li.Strong(s => s.Text("SSE (Bifrost): "));
					li.Text("The click calls a server action that publishes toast HTML to topic ");
					li.Code(c => c.Text("toasts"));
					li.Text(". Any connected page subscribed to that topic receives the toast.");
				});

				ul.Li(li =>
				{
					li.Strong(s => s.Text("OOB Invocation: "));
					li.Text("The click calls a server action that returns an ");
					li.Code(c => c.Text("<invocation>"));
					li.Text(" targeting ");
					li.Code(c => c.Text("#toast-manager"));
					li.Text(". Heimdall.js applies it client-side without rendering the invocation element.");
				});
			});
		});

		public static IHtmlContent Render()
		{
			var card = FluentHtml.Div(d =>
			{
				d.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

				d.Div(body =>
				{
					body.Class(
						Bootstrap.Card.Body,
						Bootstrap.Text.Align(Bootstrap.TextAlignKind.Center)
					);

					body.H1(h => h.Text("Send a toast!"));
					body.P(p => p.Text("Use either approach below. Both ultimately insert a toast into the layout’s #toast-manager."));

					body.Div(btnRow =>
					{
						btnRow.Class(Bootstrap.Spacing.Mt(3));
						btnRow.Add(BtnSse, BtnOob);
					});

					body.Add(Explainer);
				});
			});

			return FluentHtml.Div(frame =>
			{
				frame.Class(
					Bootstrap.Spacing.M(3, Bootstrap.Side.Top),
					Bootstrap.Layout.ContainerFluid
				);

				frame.Div(row =>
				{
					row.Class(Bootstrap.Layout.Row);

					row.Div(col => col.Class(Bootstrap.Layout.Col));

					row.Div(center =>
					{
						center.Class(Bootstrap.Layout.ColSpan(6, Bootstrap.Breakpoint.Lg));
						center.Add(card);
					});

					row.Div(col => col.Class(Bootstrap.Layout.Col));
				});
			});
		}

		[ContentInvocation]
		public static async Task<IHtmlContent> ToastSSEHello(Bifrost bifrost)
		{
			var toast = new ToastItem
			{
				Header = "Hello from SSE!",
				Content = "This toast was published to the 'toasts' topic and delivered over EventSource.",
				Type = ToastType.Success,
				DurationMs = 1800
			};

			var html = ToastManager.Create(toast);

			await bifrost.PublishAsync("toasts", html, TimeSpan.FromSeconds(10));

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
