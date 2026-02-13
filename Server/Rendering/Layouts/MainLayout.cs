using Microsoft.AspNetCore.Html;
using Server.Rendering.Shared;
using Server.Utilities;

namespace Server.Rendering.Layouts
{
    public static class MainLayout
    {
        public static IHtmlContent Render(HttpContext ctx, IHtmlContent page, string title,bool enableSSE = true)
        {
            return Html.Fragment(
                Html.Raw("<!DOCTYPE html>"),

                Html.HtmlTag(
                    Html.Attr("lang", "en"),

                    Html.Head(
                        Html.Meta(
                            Html.Attr("charset", "utf-8")
                        ),

                        Html.Meta(
                            Html.Name("viewport"),
                            Html.Content("width=device-width, initial-scale=1")
                        ),

                        Html.Title(title),
                        SeoFragment.Twitter,
                        SeoFragment.OpenGraph,
                        SeoFragment.Default,

                        Html.Link(
                            Html.Attr("rel", "icon"),
                            Html.Type("image/png"),
                            Html.Href("images/favicon.png")
                        ),

                        Html.Link(
                            Html.Attr("rel", "stylesheet"),
                            Html.Href("css/app.css")
                        ),

                        Html.Link(
                            Html.Attr("rel", "stylesheet"),
                            Html.Href("css/bootstrap.css")
                        ),

                        Html.Link(
                            Html.Attr("rel", "stylesheet"),
                            Html.Href("css/bootstrap-icons.css")
                        ),

                        Html.Script(
                            Html.Src("/_content/HeimdallFramework.Web/heimdall.js")
                        ),

                        Html.Script(
                            Html.Src("js/bootstrap-bundle.js")
                        )
                    ),

                    Html.Body(
                        ToastManager.Render(ctx, enableSSE),
                        MenuComponent.Render(ctx),

                        Html.Div(
                            Html.Class("container-fluid", "p-2"),
                            page
                        )
                    )
                )
            );
        }
    }
}
