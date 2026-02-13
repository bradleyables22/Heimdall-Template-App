using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Rendering.Shared;
using Server.Utilities;

namespace Server.Rendering.Pages
{
    public class OobPage
    {
        // Adjust to match your action-id resolver.
        private static readonly HeimdallHtml.ActionId ActionToastSse = "OobPage.ToastSSEHello";
        private static readonly HeimdallHtml.ActionId ActionToastOob = "OobPage.ToastHello";

        public static IHtmlContent Render()
        {
            var btnSse = Html.Button(
                Html.Type("button"),
                Html.Class(Bootstrap.Btn.Primary, Bootstrap.Spacing.Me(2)),
                HeimdallHtml.OnClick(ActionToastSse),
                HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
                HeimdallHtml.PayloadEmptyObject(),
                Html.Text("Send toast via SSE")
            );

            var btnOob = Html.Button(
                Html.Type("button"),
                Html.Class(Bootstrap.Btn.OutlinePrimary),
                HeimdallHtml.OnClick(ActionToastOob),
                HeimdallHtml.SwapMode(HeimdallHtml.Swap.None),
                HeimdallHtml.PayloadEmptyObject(),
                Html.Text("Send toast via OOB Invocation")
            );

            var explainer = Html.Div(
                Html.Class(/*Bootstrap.Alert.Base, */Bootstrap.Alert.Variant(Bootstrap.Color.Light), Bootstrap.Spacing.Mt(3), Bootstrap.Text.Align(Bootstrap.TextAlignKind.Start)),
                Html.H5("Where the toast manager comes from"),
                Html.P(
                    Html.Text("This page does not render the toast container. "),
                    Html.Strong("The layout is responsible for rendering "),
                    Html.Code("#toast-manager"),
                    Html.Text(" (and, if enabled, subscribing it to the "),
                    Html.Code("toasts"),
                    Html.Text(" topic via SSE).")
                ),
                Html.Hr(),
                Html.H6("Two methodologies"),
                Html.Ul(
                    Html.Li(
                        Html.Strong("SSE (Bifrost): "),
                        Html.Text("The click calls a server action that publishes toast HTML to topic "),
                        Html.Code("toasts"),
                        Html.Text(". Any connected page subscribed to that topic receives the toast.")
                    ),
                    Html.Li(
                        Html.Strong("OOB Invocation: "),
                        Html.Text("The click calls a server action that returns an "),
                        Html.Code("<invocation>"),
                        Html.Text(" targeting "),
                        Html.Code("#toast-manager"),
                        Html.Text(". Heimdall.js applies it client-side without rendering the invocation element.")
                    )
                )
            );

            var card = Html.Div(
                Html.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg),
                Html.Div(
                    Html.Class(Bootstrap.Card.Body, Bootstrap.Text.Align(Bootstrap.TextAlignKind.Center)),
                    Html.H1("Send a toast!"),
                    Html.P("Use either approach below. Both ultimately insert a toast into the layout’s #toast-manager."),
                    Html.Div(
                        Html.Class(Bootstrap.Spacing.Mt(3)),
                        btnSse,
                        btnOob
                    ),
                    explainer
                )
            );

            return Html.Div(
                Html.Class(Bootstrap.Spacing.M(3, Bootstrap.Side.Top), Bootstrap.Layout.ContainerFluid),
                Html.Div(
                    Html.Class(Bootstrap.Layout.Row),
                    Html.Div(Html.Class(Bootstrap.Layout.Col)),
                    Html.Div(
                        Html.Class(Bootstrap.Layout.ColSpan(6, Bootstrap.Breakpoint.Lg)),
                        card
                    ),
                    Html.Div(Html.Class(Bootstrap.Layout.Col))
                )
            );
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

            // Publish for any subscribers (layout typically subscribes #toast-manager)
            await bifrost.PublishAsync("toasts", html, TimeSpan.FromSeconds(10));

            // Nothing to swap into the clicked element.
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

            // OOB instruction: insert newest first
            IHtmlContent result = Html.Fragment(
                HeimdallHtml.Invocation(
                    targetSelector: "#toast-manager",
                    swap: HeimdallHtml.Swap.AfterBegin,
                    payload: html,
                    wrapInTemplate: false
                )
            );

            return Task.FromResult(result);
        }
    }
}
