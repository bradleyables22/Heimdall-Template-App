using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Rendering.Shared
{
    public static class FeatureCard
    {
        public static IHtmlContent Render(string icon, string title, string body)
            => Html.Div(
                Html.Class(
                    Bootstrap.Layout.ColSpan(12),
                    Bootstrap.Layout.ColSpan(4, Bootstrap.Breakpoint.Lg)
                ),
                Html.Div(
                    Html.Class(
                        Bootstrap.Border.Default,
                        Bootstrap.Border.Rounded,
                        Bootstrap.Spacing.P(3),
                        Bootstrap.Sizing.H100
                    ),
                    Html.Div(
                        Html.Class(
                            Bootstrap.Display.Flex,
                            Bootstrap.Spacing.Gap(2)
                        ),
                        Html.I(
                            Html.Class(
                                Bootstrap.Raw(icon),
                                Bootstrap.Raw("fs-4")
                            )
                        ),
                        Html.Div(
                            Html.H4(
                                Html.Class(
                                    Bootstrap.Raw("h6"),
                                    Bootstrap.Spacing.Mb(1)
                                ),
                                Html.Text(title)
                            ),
                            Html.P(
                                Html.Class(
                                    Bootstrap.Spacing.Mb(0),
                                    Bootstrap.Text.BodySecondary
                                ),
                                Html.Text(body)
                            )
                        )
                    )
                )
            );

    }
}
