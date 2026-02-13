using Microsoft.AspNetCore.Html;
using Server.Rendering.Shared;
using Server.Utilities;

namespace Server.Rendering.Pages
{
    public static class MainPage
    {
        public static IHtmlContent Render()
        {
            var hero = Html.Div(
                Html.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg),
                Html.Div(
                    Html.Class(Bootstrap.Card.Body),
                    HeaderRow,
                    Html.Hr(),
                    Features
                )
            );

            // Page frame
            return Html.Div(
                Html.Class(Bootstrap.Spacing.Mt(3)),
                Html.Div(
                    Html.Class(Bootstrap.Layout.Row),
                    Html.Div(Html.Class(Bootstrap.Layout.Col)),
                    Html.Div(
                        Html.Class(Bootstrap.Layout.ColSpan(8, Bootstrap.Breakpoint.Lg)),
                        hero
                    ),
                    Html.Div(Html.Class(Bootstrap.Layout.Col))
                )
            );
        }

        private static IHtmlContent Features = Html.Div(
                Html.Class(
                    Bootstrap.Layout.Row,
                    Bootstrap.Layout.Gutter(3)
                ),
                FeatureCard.Render(
                    icon: "bi bi-lightning-charge-fill",
                    title: "Server actions",
                    body: "Trigger backend methods with simple attributes and swap the returned HTML into the DOM."
                ),
                FeatureCard.Render(
                    icon: "bi bi-arrow-left-right",
                    title: "Swap modes",
                    body: "inner, outer, afterbegin, beforeend… keep updates tight and fast with predictable DOM changes."
                ),
                FeatureCard.Render(
                    icon: "bi bi-broadcast-pin",
                    title: "Optional SSE (Bifrost)",
                    body: "Push HTML updates to subscribed clients with topic-based streams—no websockets required."
                )
            );

        private static IHtmlContent HeaderRow = Html.Div(
                Html.Class(
                    Bootstrap.Display.Flex,
                    Bootstrap.Flex.AlignItemsCenter,
                    Bootstrap.Flex.JustifyBetween,
                    Bootstrap.Flex.Wrap,
                    Bootstrap.Spacing.Gap(3)
                ),
                Html.Div(
                    Html.Class(Bootstrap.Text.Start),
                    Html.H1(
                        Html.Class(
                            Bootstrap.Raw("display-6"),
                            Bootstrap.Spacing.Mb(2)
                        ),
                        Html.I(
                            Html.Class(
                                Bootstrap.Raw("bi bi-shield-lock-fill"),
                                Bootstrap.Spacing.Me(2)
                            )
                        ),
                        Html.Text("Welcome to Heimdall")
                    ),
                    Html.P(
                        Html.Class(
                            Bootstrap.Text.BodySecondary,
                            Bootstrap.Spacing.Mb(0)
                        ),
                        Html.Text("HTML-first server actions + safe DOM swapping + optional SSE. "),
                        Html.Text("Build fast pages without a heavy client framework.")
                    )
                )
            );
    }
}
