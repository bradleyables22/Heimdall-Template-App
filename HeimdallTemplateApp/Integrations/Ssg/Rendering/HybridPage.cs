using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;

namespace HeimdallTemplateApp.Rendering;

public static class HybridPage
{
    public static IHtmlContent Render(SiteRenderContext site)
        => FluentHtml.Fragment(page =>
        {
            var content = site.Content;

            page.Section(section =>
            {
                section.Class("py-5 border-bottom")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row align-items-center g-5")
                                    .Div(copy =>
                                    {
                                        copy.Class("col-lg-6")
                                            .P(p => p.Class("eyebrow text-primary fw-semibold mb-2").Text("Hybrid documentation page"))
                                            .H1(h => h.Class("display-5 fw-bold mb-3").Text("Static Pages With Runtime Sections"))
                                            .P(p => p.Class("lead text-secondary mb-4").Text("The shell below is generated at build time. When the ASP.NET Core app is running, Heimdall loads a small reference panel into the page without replacing the static document."))
                                            .Div(actions =>
                                            {
                                                actions.Class("d-flex flex-wrap gap-2")
                                                    .A(a => a.Class("btn btn-primary").Href(site.ToSitePath("/mvc-view/")).Text("View MVC example"))
                                                    .A(a => a.Class("btn btn-outline-dark").Href(site.ToSitePath("/markdown/")).Text("View markdown example"));
                                            });
                                    })
                                    .Div(panel =>
                                    {
                                        panel.Class("col-lg-6")
                                            .Div(card =>
                                            {
                                                card.Id("runtime-reference-panel")
                                                    .Class("card border-0 shadow-sm")
                                                    .Add(HeimdallHtml.OnLoad("hybrid.reference"))
                                                    .Add(HeimdallHtml.Target("#runtime-reference-panel"))
                                                    .Add(HeimdallHtml.SwapMode(HeimdallHtml.Swap.Inner))
                                                    .Div(cardBody =>
                                                    {
                                                        cardBody.Class("card-body p-4")
                                                            .Div(status =>
                                                            {
                                                                status.Class("d-flex align-items-center gap-3")
                                                                    .Div(spinner =>
                                                                    {
                                                                        spinner.Class("spinner-border text-primary")
                                                                            .Role("status")
                                                                            .Span(span => span.Class("visually-hidden").Text("Loading"));
                                                                    })
                                                                    .Div(copy =>
                                                                    {
                                                                        copy.H2(h => h.Class("h5 mb-1").Text("Loading runtime reference"))
                                                                            .P(p => p.Class("text-secondary mb-0").Text("If this page is hosted as plain static HTML, this fallback remains visible. Running the app enables Heimdall hydration."));
                                                                    });
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

                                foreach (var item in content.HybridNotes)
                                {
                                    row.Div(col =>
                                    {
                                        col.Class("col-md-4")
                                            .Article(card =>
                                            {
                                                card.Class("h-100 p-4 bg-white border rounded-3")
                                                    .H2(h => h.Class("h5 fw-bold").Text(item.Title))
                                                    .P(p => p.Class("text-secondary mb-0").Text(item.Description));
                                            });
                                    });
                                }
                            });
                    });
            });
        });
}
