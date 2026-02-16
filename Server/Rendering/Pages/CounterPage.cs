using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;

namespace Server.Pages
{
    public static class CounterPage
    {
        public static class Actions
        {
            public const string Inc = "CounterPage.Inc";
            public const string Dec = "CounterPage.Dec";
            public const string Reset = "CounterPage.Reset";
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
                host.Id("counter-host");
                host.Class(
                    Bootstrap.Card.Base,
                    Bootstrap.Shadow.Sm,
                    Bootstrap.Spacing.P(3),
                    Bootstrap.Display.InlineBlock
                );

                // State stored in DOM (read by Heimdall.js closest-state)
                host.Add(HeimdallHtml.State(new CounterState(count)));

                host.Div(row =>
                {
                    row.Class(
                        Bootstrap.Display.Flex,
                        Bootstrap.Flex.AlignItemsCenter,
                        Bootstrap.Flex.JustifyBetween,
                        Bootstrap.Spacing.Gap(3)
                    );

                    row.Div(left =>
                    {
                        left.Div(lbl => lbl.Class(Bootstrap.Text.BodySecondary, Bootstrap.Text.Small).Text("Count"));
                        left.Div(val => val.Class(Bootstrap.Raw("display-6"), Bootstrap.Spacing.Mb(0)).Text(count.ToString()));
                    });

                    row.Div(btns =>
                    {
                        btns.Class(Bootstrap.Display.Flex, Bootstrap.Spacing.Gap(2));

                        btns.Button(b =>
                        {
                            b.Class(Bootstrap.Btn.OutlineSecondary);
                            b.Type("button");
                            b.Text("−");

                            b.Heimdall()
                                .Click(Actions.Dec)
                                .PayloadFromClosestState()
                                .Target("#counter-host")
                                .SwapOuter();
                        });

                        btns.Button(b =>
                        {
                            b.Class(Bootstrap.Btn.Primary);
                            b.Type("button");
                            b.Text("+");

                            b.Heimdall()
                                .Click(Actions.Inc)
                                .PayloadFromClosestState()
                                .Target("#counter-host")
                                .SwapOuter();
                        });

                        btns.Button(b =>
                        {
                            b.Class(Bootstrap.Btn.OutlineDanger);
                            b.Type("button");
                            b.Text("Reset");

                            b.Heimdall()
                                .Click(Actions.Reset)
                                .PayloadFromClosestState()
                                .Target("#counter-host")
                                .SwapOuter();
                        });
                    });
                });
            });
        }

        [ContentInvocation]
        public static IHtmlContent Inc(CounterState state)
            => RenderCounterHost((state?.Count ?? 0) + 1);

        [ContentInvocation]
        public static IHtmlContent Dec(CounterState state)
            => RenderCounterHost((state?.Count ?? 0) - 1);

        [ContentInvocation]
        public static IHtmlContent Reset(CounterState state)
            => RenderCounterHost(0);
    }
}
