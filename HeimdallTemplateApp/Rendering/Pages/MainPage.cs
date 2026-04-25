using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Rendering.Shared;
using Heimdall.Bootstrap;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Pages
{
    public static class MainPage
    {
        private const string LoadActionId = "MainPage.RenderBody";
        private const string HostId = "mainpage-host";
		public static IHtmlContent Render()
        {
            return FluentHtml.Div(host =>
            {
                host.Id(HostId)
                .Heimdall()
                    .Load(LoadActionId)
                    .Target($"#{HostId}")
                    .SwapInner();

                host.Div(s =>
                {
                    s.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Sm)
                    .Div(b =>
                    {
                        b.Class(Bootstrap.Card.Body)
                        .Div(r =>
                        {
                            r.Class(Bootstrap.Display.Flex, Bootstrap.Flex.AlignItemsCenter, Bootstrap.Spacing.Gap(2))
                            .Div(spinner =>
                            {
                                spinner.Class("spinner-border")
                                .Attr("role", "status")
                                .Span(sr =>
                                {
                                    sr.Class("visually-hidden")
                                    .Text("Loading...");
                                });
                            })
                            .Div(txt =>
                            {
                                txt.Class(Bootstrap.Text.BodySecondary)
                                .Text("Loading Heimdall…");
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
                card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg)
                .Div(body =>
                {
                    body.Class(Bootstrap.Card.Body)
                    .Add(HeaderRow)
                    .Hr(_ => { })
                    .Add(Features);
                });
            });

            return FluentHtml.Fragment(frame =>
            {
                frame.Div(row =>
                {
                    row.Class(Bootstrap.Layout.Row, Bootstrap.Spacing.Mt(3))
                    .Div(_ => { _.Class(Bootstrap.Layout.Col); })
                    .Div(center =>
                    {
                        center.Class(Bootstrap.Layout.ColSpan(8, Bootstrap.Breakpoint.Lg))
                        .Add(hero);
                    })
                    .Div(_ => { _.Class(Bootstrap.Layout.Col); });
                });
            });
        }

        private static IHtmlContent Features
            => FluentHtml.Div(row =>
            {
                row.Class(
                    Bootstrap.Layout.Row,
                    Bootstrap.Layout.Gutter(3)
                )
                .Add(
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
                )
                .Div(left =>
                {
                    left.Class(Bootstrap.Text.Start)
                    .H1(h =>
                    {
                        h.Class(
                            Bootstrap.Raw("display-6"),
                            Bootstrap.Spacing.Mb(2)
                        )
                        .I(i =>
                        {
                            i.Class(
                                Bootstrap.Raw("bi bi-shield-lock-fill"),
                                Bootstrap.Spacing.Me(2)
                            );
                        })
                        .Text("Welcome to Heimdall");
                    })
                    .P(p =>
                    {
                        p.Class(
                            Bootstrap.Text.BodySecondary,
                            Bootstrap.Spacing.Mb(0)
                        )
                        .Text("HTML-first server actions + safe DOM swapping + optional SSE. ")
                        .Text("Build fast pages without a heavy client framework.");
                    });
                });
            });
    }
}
