using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;

namespace HeimdallTemplateApp.Rendering;

public static class NotFoundPage
{
    public static IHtmlContent Render(SiteRenderContext site)
        => FluentHtml.Fragment(page =>
        {
            page.Section(hero =>
            {
                hero.Class("py-5")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row justify-content-center text-center")
                                    .Div(col =>
                                    {
                                        col.Class("col-lg-7")
                                            .P(p => p.Class("eyebrow text-primary fw-semibold mb-2").Text("404"))
                                            .H1(h => h.Class("display-5 fw-bold mb-3").Text("That page is not generated"))
                                            .P(p => p.Class("lead text-secondary mb-4").Text("This template generates a static 404.html file so documentation hosts can use the common fallback convention."))
                                            .Div(actions =>
                                            {
                                                actions.Class("d-flex justify-content-center flex-wrap gap-2")
                                                    .A(a => a.Class("btn btn-primary").Href(site.ToSitePath("/")).Text("Return home"));
                                            });
                                    });
                            });
                    });
            })
            .Section(section =>
            {
                section.Class("pb-5")
                    .Div(container =>
                    {
                        container.Class("container")
                            .Div(row =>
                            {
                                row.Class("row justify-content-center")
                                    .Div(col =>
                                    {
                                        col.Class("col-lg-8")
                                            .Div(note =>
                                            {
                                                note.Class("alert alert-light border")
                                                    .Strong(strong => strong.Text("Production note: "))
                                                    .Span(span => span.Text("different static hosts expose different 404 configuration knobs. Generating 404.html gives you the common denominator."));
                                            });
                                    });
                            });
                    });
            });
        });
}
