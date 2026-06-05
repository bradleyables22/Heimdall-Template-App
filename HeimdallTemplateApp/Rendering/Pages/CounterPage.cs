using Heimdall.Bootstrap;
using Heimdall.Server;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;

namespace HeimdallTemplateApp.Rendering.Pages
{
    public static class CounterPage
    {
        public const string HostId = "counter-host";

		public static class Actions
        {
            public const string Inc = "counter.inc";
            public const string Dec = "counter.dec";
            public const string Reset = "counter.reset";
        }

        public sealed class CounterState
        {
            public CounterState() { }
            public CounterState(int count) => Count = count;
            public int Count { get; set; }
        }

        public static IHtmlContent Render()
        {
            return FluentHtml.Div(root =>
            {
                root.Class(
                    Bootstrap.Layout.Container,
                    Bootstrap.Spacing.Py(4)
                );

                root.H2(h => h.Text("Counter (Heimdall + closest-state)"));

                root.P(p =>
                {
                    p.Class(Bootstrap.Text.BodySecondary);
                    p.Text("State lives on the host as data-heimdall-state. Buttons send payload-from=\"closest-state\".");
                });

                // Initial host (outer-swapped by actions)
                root.Content(RenderCounterHost(count: 0));
            });
        }

        // Host owns state
        public static IHtmlContent RenderCounterHost(int count)
        {
            return FluentHtml.Div(host =>
            {
                host.Id(HostId)
                .Class(
                    Bootstrap.Card.Base,
                    Bootstrap.Shadow.Sm,
                    Bootstrap.Spacing.P(3),
                    Bootstrap.Display.InlineBlock
                );

                // State stored in DOM (read by Heimdall.js closest-state)
                host.Add(HeimdallHtml.State(new CounterState(count)))
                .Div(row =>
                {
                    row.Class(
                        Bootstrap.Display.Flex,
                        Bootstrap.Flex.AlignItemsCenter,
                        Bootstrap.Flex.JustifyBetween,
                        Bootstrap.Spacing.Gap(3)
                    )
                    .Div(left =>
                    {
                        left.Div(lbl => lbl.Class(Bootstrap.Text.BodySecondary, Bootstrap.Text.Small).Text("Count"))
                        .Div(val => val.Class(Bootstrap.Raw("display-6"), Bootstrap.Spacing.Mb(0)).Text(count.ToString()));
                    })
                    .Div(btns =>
                    {
                        btns.Class(Bootstrap.Display.Flex, Bootstrap.Spacing.Gap(2))
                        .Button(b =>
                        {
                            b.Class(Bootstrap.Btn.OutlineSecondary)
                            .Type("button")
                            .Text("−")
                            .Heimdall()
                                .Click(Actions.Dec)
                                .PayloadFromClosestState()
                                .Target($"#{HostId}")
                                .SwapOuter();
                        })
                        .Button(b =>
                        {
                            b.Class(Bootstrap.Btn.Primary)
                            .Type("button")
                            .Text("+")

                            .Heimdall()
                                .Click(Actions.Inc)
                                .PayloadFromClosestState()
                                .Target($"#{HostId}")
                                .SwapOuter();
                        })
                        .Button(b =>
                        {
                            b.Class(Bootstrap.Btn.OutlineDanger)
                            .Type("button")
                            .Text("Reset")
                            .Heimdall()
                                .Click(Actions.Reset)
                                .PayloadFromClosestState()
                                .Target($"#{HostId}")
                                .SwapOuter();
                        });
                    });
                });
            });
        }

    }

    [ContentInvocationPrefix("counter")]
    public sealed class CounterActions
    {
        [ContentInvocation("inc")]
        [RequestTimeout(3000)]
        public IHtmlContent Inc([ContentPayload] CounterPage.CounterState state)
            => CounterPage.RenderCounterHost((state?.Count ?? 0) + 1);

        [ContentInvocation("dec")]
        [RequestTimeout(3000)]
        public IHtmlContent Dec([ContentPayload] CounterPage.CounterState state)
            => CounterPage.RenderCounterHost((state?.Count ?? 0) - 1);

        [ContentInvocation("reset")]
        [RequestTimeout(3000)]
        public IHtmlContent Reset([ContentPayload] CounterPage.CounterState state)
            => CounterPage.RenderCounterHost(0);
    }
}
