using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;

namespace HeimdallTemplateApp.Rendering;

public static class HomePage
{
    public static IHtmlContent Render(SiteRenderContext site)
        => FluentHtml.Fragment(page =>
        {
            var content = site.Content;

            page.Section(hero =>
            {
                hero.Class("hero-band py-5 py-lg-6 border-bottom")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row align-items-center g-5")
                                    .Div(copy =>
                                    {
                                        copy.Class("col-lg-7")
                                            .P(p => p.Class("eyebrow text-primary fw-semibold mb-2").Text("Fluent HTML documentation page"))
                                            .H1(h => h.Class("display-4 fw-bold lh-1 mb-4").Text(content.ProductName))
                                            .P(p => p.Class("lead text-secondary mb-4").Text(content.Description))
                                            .Div(actions =>
                                            {
                                                actions.Class("d-flex flex-wrap gap-2")
                                                    .A(a => a.Class("btn btn-primary btn-lg").Href(site.ToSitePath("/markdown/")).Text("Read markdown docs"));
                                            });
                                    })
                                    .Div(panel =>
                                    {
                                        panel.Class("col-lg-5")
                                            .Div(card =>
                                            {
                                                card.Class("card shadow-sm border-0")
                                                    .Div(cardBody =>
                                                    {
                                                        cardBody.Class("card-body p-4")
                                                            .Div(header =>
                                                            {
                                                                header.Class("d-flex align-items-center justify-content-between mb-4")
                                                                    .Span(span => span.Class("badge text-bg-primary").Text("Docs"))
                                                                    .Span(span => span.Class("text-secondary small").Text("Build-time generated"));
                                                            })
                                                            .H2(h => h.Class("h4 fw-bold").Text("Generation pipeline"))
                                                            .P(p => p.Class("text-secondary").Text("This template documents the SSG flow directly in the generated site: register routes, render with scoped services, and copy assets for static hosting."))
                                                            .Div(metrics =>
                                                            {
                                                                metrics.Class("row g-3 mt-2");

                                                                foreach (var metric in content.HeroMetrics)
                                                                {
                                                                    metrics.Div(col =>
                                                                    {
                                                                        col.Class("col-6")
                                                                            .Div(tile =>
                                                                            {
                                                                                tile.Class("metric-tile")
                                                                                    .Span(span => span.Class("metric-value").Text(metric.Value))
                                                                                    .Span(span => span.Class("metric-label").Text(metric.Label));
                                                                            });
                                                                    });
                                                                }
                                                            });
                                                    });
                                            });
                                    });
                            });
                    });
            })
            .Section(section =>
            {
                section.Class("py-5")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row g-4");

                                foreach (var feature in content.Features)
                                {
                                    row.Div(col =>
                                    {
                                        col.Class("col-md-6 col-lg-3")
                                            .Article(card =>
                                            {
                                                card.Class("h-100 p-4 border rounded-3 bg-white")
                                                    .Span(span => span.Class("feature-kicker").Text(feature.Kicker))
                                                    .H2(h => h.Class("h5 fw-bold mt-3").Text(feature.Title))
                                                    .P(p => p.Class("text-secondary mb-0").Text(feature.Description));
                                            });
                                    });
                                }
                            });
                    });
            })
            .Section(section =>
            {
                section.Class("py-5 bg-white border-top")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row align-items-center g-4")
                                    .Div(copy =>
                                    {
                                        copy.Class("col-lg-6")
                                            .H2(h => h.Class("display-6 fw-bold").Text("Use this as living documentation"))
                                            .P(p => p.Class("text-secondary").Text("Each route explains one production pattern while also exercising it. The page you are reading is fluent HTML, the MVC page comes from Razor, and the markdown page comes from Markdig."))
                                            .Pre(pre =>
                                            {
                                                pre.Class("docs-code")
                                                    .Code(code => code.Text("""
builder.Services
    .AddHeimdallStaticSiteGeneration(options => options.UseWebRootPath())
    .WithStaticPage("/", ctx => RenderAssistant.RenderPage(ctx, "Heimdall SSG Docs", "/", HomePage.Render));
"""));
                                            });
                                    })
                                    .Div(list =>
                                    {
                                        list.Class("col-lg-6")
                                            .Div(group =>
                                            {
                                                group.Class("list-group list-group-flush border rounded-3");

                                                foreach (var path in content.RouteExamples)
                                                {
                                                    group.A(a =>
                                                    {
                                                        a.Class("list-group-item list-group-item-action d-flex justify-content-between align-items-center")
                                                            .Href(site.ToSitePath(path.Href))
                                                            .Span(span => span.Text(path.Label))
                                                            .Span(span => span.Class("small text-secondary").Text(path.Renderer));
                                                    });
                                                }
                                            });
                                    });
                            });
                    });
            });
        });
}
