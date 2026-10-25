using Heimdall.Server;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Services;

namespace HeimdallTemplateApp.ContentActions;

[ContentInvocationPrefix("hybrid")]
public sealed class HybridContentActions(SiteContent content)
{
    [ContentInvocation("reference")]
    public IHtmlContent Reference()
        => FluentHtml.Fragment(fragment =>
        {
            fragment.Div(header =>
            {
                header.Class("card-body p-4 border-bottom")
                    .Div(row =>
                    {
                        row.Class("d-flex flex-wrap justify-content-between align-items-start gap-3")
                            .Div(copy =>
                            {
                                copy.Span(span => span.Class("badge text-bg-success mb-2").Text("Live"))
                                    .H2(h => h.Class("h4 fw-bold mb-1").Text("Runtime reference loaded"))
                                    .P(p => p.Class("text-secondary mb-0").Text("Rendered by a Heimdall content action after the static documentation shell loads."));
                            })
                            .Div(meta =>
                            {
                                meta.Class("text-end")
                                    .Div(value => value.Class("fs-3 fw-bold").Text(content.RuntimeReference.Status))
                                    .Span(span => span.Class("small text-secondary").Text("load status"));
                            });
                    });
            })
            .Div(body =>
            {
                body.Class("card-body p-4")
                    .Div(metrics =>
                    {
                        metrics.Class("row g-3 mb-4");

                        foreach (var metric in content.RuntimeReference.Metrics)
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
                    })
                    .H3(h => h.Class("h6 text-uppercase text-secondary fw-semibold").Text("Attributes used"))
                    .Ul(list =>
                    {
                        list.Class("list-unstyled mb-0");

                        foreach (var action in content.RuntimeReference.Actions)
                        {
                            list.Li(item =>
                            {
                                item.Class("d-flex gap-2 mb-2")
                                    .Span(span => span.Class("text-primary").Text("-"))
                                    .Span(span => span.Class("text-break").Text(action));
                            });
                        }
                    });
            });
        });
}
