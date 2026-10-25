using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;

namespace HeimdallTemplateApp.Rendering;

public static class SiteLayout
{
    private static readonly IReadOnlyList<NavigationItem> Navigation =
    [
        new("/", "Overview"),
        new("/mvc-view/", "MVC"),
        new("/markdown/", "Markdown"),
        new("/hybrid/", "Hybrid")
    ];

    public static IHtmlContent Render(
        string title,
        string activePath,
        Func<string, string> toSitePath,
        IHtmlContent body)
        => FluentHtml.Fragment(document =>
        {
            document.Raw("<!DOCTYPE html>")
                .HtmlTag(html =>
                {
                    html.Attr("lang", "en")
                        .Head(head =>
                        {
                            head.Meta(m => m.Attr("charset", "utf-8"))
                                .Meta(m =>
                                {
                                    m.Name("viewport")
                                        .ContentAttr("width=device-width, initial-scale=1");
                                })
                                .Title(t => t.Text(title))
                                .Link(l =>
                                {
                                    l.Attr("rel", "icon")
                                        .Type("image/png")
                                        .Href(toSitePath("/Images/Favicon.png"));
                                })
                                .Link(l =>
                                {
                                    l.Attr("rel", "stylesheet")
                                        .Href(toSitePath("/css/vendor/bootstrap.css"));
                                })
                                .Link(l =>
                                {
                                    l.Attr("rel", "stylesheet")
                                        .Href(toSitePath("/css/vendor/bootstrap-icons.css"));
                                })
                                .Link(l =>
                                {
                                    l.Attr("rel", "stylesheet")
                                        .Href(toSitePath("/css/site.css"));
                                });
                        })
                        .Body(bodyBuilder =>
                        {
                            bodyBuilder.Header(header =>
                            {
                                header.Class("border-bottom bg-white sticky-top")
                                    .Nav(nav =>
                                    {
                                        nav.Class("navbar navbar-expand-lg")
                                            .Attr("aria-label", "Primary")
                                            .Div(container =>
                                            {
                                                container.Class("container")
                                                    .A(a =>
                                                    {
                                                        a.Class("navbar-brand fw-bold")
                                                            .Href(toSitePath("/"))
                                                            .Text("Heimdall SSG Docs");
                                                    })
                                                    .Div(links =>
                                                    {
                                                        links.Class("navbar-nav flex-row flex-wrap gap-2 ms-lg-auto");

                                                        foreach (var item in Navigation)
                                                        {
                                                            links.A(a =>
                                                            {
                                                                var isActive = IsActive(activePath, item.Href);
                                                                a.Href(toSitePath(item.Href))
                                                                    .Class(isActive
                                                                        ? "nav-link px-2 active fw-semibold"
                                                                        : "nav-link px-2");

                                                                if (isActive)
                                                                    a.Attr("aria-current", "page");

                                                                a.Text(item.Label);
                                                            });
                                                        }
                                                    });
                                            });
                                    });
                            })
                            .Main(main =>
                            {
                                main.Class("site-main")
                                    .Add(body);
                            })
                            .Footer(footer =>
                            {
                                footer.Class("border-top bg-white py-4")
                                    .Div(container =>
                                    {
                                        container.Class("container d-flex flex-column flex-md-row gap-2 justify-content-between text-secondary small")
                                            .Span(span => span.Text("Generated documentation with Heimdall static site generation."))
                                            .Span(span => span.Text("Fluent HTML, MVC views, markdown, and hybrid load examples."));
                                    });
                            })
                            .Script(script =>
                            {
                                script.Src(toSitePath("/js/vendor/bootstrap-bundle.js"));
                            })
                            .Script(script =>
                            {
                                script.Src(toSitePath("/_content/HeimdallFramework.Web/heimdall-bundle.min.js"));
                            });
                        });
                });
        });

    private static bool IsActive(string activePath, string href)
        => string.Equals(NormalizePath(activePath), NormalizePath(href), StringComparison.OrdinalIgnoreCase);

    private static string NormalizePath(string path)
    {
        var normalized = string.IsNullOrWhiteSpace(path) ? "/" : path.Trim();
        if (normalized != "/")
            normalized = normalized.TrimEnd('/');

        return normalized.Length == 0 ? "/" : normalized;
    }

    private sealed record NavigationItem(string Href, string Label);
}
