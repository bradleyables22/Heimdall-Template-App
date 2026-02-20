using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Heimdall;
using HeimdallTemplateApp.Rendering.Shared;
using HeimdallTemplateApp.Utilities;

namespace HeimdallTemplateApp.Rendering.Pages
{
    public static class MainPage
    {
        private const string LoadActionId = "MainPage.RenderBody";

        public static IHtmlContent Render()
        {
            return FluentHtml.Div(host =>
            {
                host.Id("mainpage-host");

                host.Heimdall()
                    .Load(LoadActionId)
                    .Target("#mainpage-host")
                    .SwapInner();

                host.Div(s =>
                {
                    s.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Sm);

                    s.Div(b =>
                    {
                        b.Class(Bootstrap.Card.Body);

                        b.Div(r =>
                        {
                            r.Class(Bootstrap.Display.Flex, Bootstrap.Flex.AlignItemsCenter, Bootstrap.Spacing.Gap(2));
                            r.Div(spinner =>
                            {
                                spinner.Class("spinner-border");
                                spinner.Attr("role", "status");
                                spinner.Span(sr =>
                                {
                                    sr.Class("visually-hidden");
                                    sr.Text("Loading...");
                                });
                            });
                            r.Div(txt =>
                            {
                                txt.Class(Bootstrap.Text.BodySecondary);
                                txt.Text("Loading Heimdall…");
                            });
                        });
                    });
                });
            });
        }

        [ContentInvocation]
        public static IHtmlContent RenderBody()
        {
            var hero = FluentHtml.Div(card =>
            {
                card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

                card.Div(body =>
                {
                    body.Class(Bootstrap.Card.Body);
                    body.Add(HeaderRow);
                    body.Hr(_ => { });
                    body.Add(Features);
                });
            });

            return FluentHtml.Fragment(frame =>
            {
                frame.Div(row =>
                {
                    row.Class(Bootstrap.Layout.Row, Bootstrap.Spacing.Mt(3));

                    row.Div(_ => { _.Class(Bootstrap.Layout.Col); });

                    row.Div(center =>
                    {
                        center.Class(Bootstrap.Layout.ColSpan(8, Bootstrap.Breakpoint.Lg));
                        center.Add(hero);
                    });

                    row.Div(_ => { _.Class(Bootstrap.Layout.Col); });
                });
            });
        }

        private static IHtmlContent Features
            => FluentHtml.Div(row =>
            {
                row.Class(
                    Bootstrap.Layout.Row,
                    Bootstrap.Layout.Gutter(3)
                );

                row.Add(
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
            });

        private static IHtmlContent HeaderRow
            => FluentHtml.Div(row =>
            {
                row.Class(
                    Bootstrap.Display.Flex,
                    Bootstrap.Flex.AlignItemsCenter,
                    Bootstrap.Flex.JustifyBetween,
                    Bootstrap.Flex.Wrap,
                    Bootstrap.Spacing.Gap(3)
                );

                row.Div(left =>
                {
                    left.Class(Bootstrap.Text.Start);

                    left.H1(h =>
                    {
                        h.Class(
                            Bootstrap.Raw("display-6"),
                            Bootstrap.Spacing.Mb(2)
                        );

                        h.I(i =>
                        {
                            i.Class(
                                Bootstrap.Raw("bi bi-shield-lock-fill"),
                                Bootstrap.Spacing.Me(2)
                            );
                        });

                        h.Text("Welcome to Heimdall");
                    });

                    left.P(p =>
                    {
                        p.Class(
                            Bootstrap.Text.BodySecondary,
                            Bootstrap.Spacing.Mb(0)
                        );

                        p.Text("HTML-first server actions + safe DOM swapping + optional SSE. ");
                        p.Text("Build fast pages without a heavy client framework.");
                    });
                });
            });
    }
}
