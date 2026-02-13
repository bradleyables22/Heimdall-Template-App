using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Rendering.Shared
{
    public static class MenuComponent
    {
        public static IHtmlContent Render(HttpContext ctx)
        {
            return Html.Fragment(
                Html.Div(
                    Html.Class(Bootstrap.Spacing.P(2),
                               Bootstrap.Flex.AlignItemsCenter, Bootstrap.Display.Flex, Bootstrap.Flex.AlignContentBetween,
                               Bootstrap.Bg.BgColor(Bootstrap.Color.Primary)),

                    Html.Div(
                        Html.Button(
                            Html.Class(Bootstrap.Btn.Base, Bootstrap.Btn.Primary, Bootstrap.Text.Uppercase),
                            Html.Attr("data-bs-toggle", "offcanvas"),
                            Html.Attr("data-bs-target", "#leftMenu"),
                            Html.Aria("controls", "leftMenu"),
                            Html.Tag("i", Html.Class("bi", "bi-grid"),Html.Class(Bootstrap.Spacing.M(1,Bootstrap.Side.End))),
                            Html.Text("Menu")
                        )
                    )
                ),

                Html.Div(
                    Html.Class(Bootstrap.Offcanvas.Base, Bootstrap.Offcanvas.Start),
                    Html.Attr("tabindex", "-1"),
                    Html.Id("leftMenu"),
                    Html.Aria("labelledby", "leftMenuLabel"),

                    Html.Div(
                        Html.Class(Bootstrap.Offcanvas.Header),

                        Html.H5(
                            Html.Class(Bootstrap.Offcanvas.Title),
                            Html.Id("leftMenuLabel"),
                            Html.Tag("i", Html.Class("bi", "bi-grid")),
                            Html.Text(" Navigation")
                        ),

                        Html.Button(
                            Html.Type("button"),
                            Html.Class("btn-close"),
                            Html.Attr("data-bs-dismiss", "offcanvas"),
                            Html.Aria("label", "Close")
                        )
                    ),

                    Html.Div(
                        Html.Class(Bootstrap.Offcanvas.Body),

                        Html.Div(
                            Html.Class(Bootstrap.ListGroup.Base),
                            Html.Id("navList"),

                            Html.A(
                                Html.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction),
                                Html.Href("/"),
                                Html.Tag("i", Html.Class("bi", "bi-house", Bootstrap.Spacing.Me(2))),
                                Html.Text(" Home")
                            ),

                            Html.A(
                                Html.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction),
                                Html.Href("/out-of-band"),
                                Html.Tag("i", Html.Class("bi", "bi-chat-dots", Bootstrap.Spacing.Me(2))),
                                Html.Text(" Out Of Band")
                            ),

                            Html.A(
                                Html.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction),
                                Html.Href("/settings"),
                                Html.Tag("i", Html.Class("bi", "bi-gear", Bootstrap.Spacing.Me(2))),
                                Html.Text(" Settings")
                            )
                        )
                    )
                ),
                Html.Script(Html.Src("components/js/menu-component.js"))
            );
        }

    }
}
