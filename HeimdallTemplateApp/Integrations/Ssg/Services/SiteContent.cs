namespace HeimdallTemplateApp.Services;

public sealed class SiteContent
{
    public string ProductName => "Heimdall Static Site Generator";

    public string Description =>
        "A runnable documentation template that shows how Heimdall renders explicit routes to static HTML while keeping ASP.NET Core DI, MVC views, and markdown content available.";

    public IReadOnlyList<FeatureItem> Features { get; } =
    [
        new(
            "Routes",
            "Explicit page registry",
            "Every generated page is registered with WithStaticPage, so builds are deterministic and do not depend on crawling a running site."),
        new(
            "DI",
            "Scoped rendering",
            "Each page render gets a fresh DI scope, which lets docs resolve options, repositories, markdown services, and MVC renderers safely."),
        new(
            "Assets",
            "Static web assets",
            "Physical wwwroot files and RCL _content assets can be copied into the output root for static hosting.")
    ];

    public IReadOnlyList<RouteExample> RouteExamples { get; } =
    [
        new("/", "SSG overview", "Fluent HTML"),
        new("/mvc-view/", "MVC static rendering", "Razor partial"),
        new("/markdown/", "Markdown-authored docs", "Markdig")
    ];

    public IReadOnlyList<Metric> HeroMetrics { get; } =
    [
        new("3", "rendering styles"),
        new("4", "generated pages"),
        new("1", "shared layout"),
        new("0", "crawler steps")
    ];

    public MvcDocsModel MvcDocs { get; } = new(
        "Render MVC Partials During Static Generation",
        "This page is generated from a Razor partial view. The static generator creates a scoped request context, renders the view through IHeimdallMvcRenderer, then writes the result as static HTML.",
        [
            new("Register MVC support", "Call AddHeimdallMvc() so view services, IHttpContextAccessor, and IHeimdallMvcRenderer are available."),
            new("Render from the page scope", "Resolve a small adapter from the static page context and call PartialAsync with the route cancellation token."),
            new("Keep views portable", "Pass a view model with generated links already mapped through ctx.ToSitePath(...) for path-base-safe output.")
        ],
        [
            new("Razor", "view engine"),
            new("Scoped", "service lifetime"),
            new("HTML", "static output")
        ]);

    public DateTimeOffset BuildTimestampUtc { get; } = DateTimeOffset.UtcNow;
}

public sealed record FeatureItem(string Kicker, string Title, string Description);

public sealed record RouteExample(string Href, string Label, string Renderer);

public sealed record Metric(string Value, string Label);

public sealed record MvcDocsModel(
    string Title,
    string Summary,
    IReadOnlyList<MvcDocsCheckpoint> Checkpoints,
    IReadOnlyList<Metric> Metrics)
{
    public string HomeHref { get; init; } = "/";
}

public sealed record MvcDocsCheckpoint(string Title, string Description);
