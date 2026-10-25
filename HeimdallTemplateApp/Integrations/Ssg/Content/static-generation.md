# Static Generation From Markdown

This page is authored as markdown, rendered with Markdig during the Heimdall static generation pass, and then wrapped in the same shared layout as the fluent HTML and MVC examples.

Use this style when the content benefits from a docs-friendly authoring workflow but still needs the application shell, Bootstrap styling, generated links, and deployment rules from the rest of the app.

## What this proves

- Markdown files can participate in the same explicit route registry as fluent pages and MVC partials.
- The renderer is resolved from DI, so teams can swap Markdig configuration, front matter parsing, CMS exports, or repository-backed content services without changing the generator contract.
- Links and local assets should still flow through the static page context when the wrapper builds navigation or layout chrome.

## Recommended shape

Keep markdown focused on document content. Let the surrounding fluent or Razor wrapper own page chrome, active navigation, metadata, and path-base-safe links.

```csharp
var markdown = ctx.GetRequiredService<MarkdownPageRenderer>();
var docs = await markdown.RenderAsync("Content/static-generation.md", ctx.CancellationToken);
var body = FluentHtml.Fragment(page => page.Add(docs));

return SiteLayout.Render("Markdown Docs", "/markdown/", ctx.ToSitePath, body);
```

## Production notes

Treat markdown as input to the same build pipeline as the rest of your generated site. Validate it in CI, keep renderer options deterministic, and fail the static generation pass when required content is missing.
