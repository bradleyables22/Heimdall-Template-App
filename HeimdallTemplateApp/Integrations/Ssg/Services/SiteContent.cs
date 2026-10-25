namespace HeimdallTemplateApp.Services;

public sealed class SiteContent
{
    public string ProductName => "Heimdall Static Site Generation";

    public string Description =>
        "A runnable documentation template that shows how Heimdall renders explicit routes to static HTML while keeping ASP.NET Core DI, MVC views, markdown content, and live Heimdall content actions available.";

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
            "Physical wwwroot files and RCL _content assets can be copied into the output root for static hosting."),
        new(
            "Hybrid",
            "Runtime islands",
            "Generated pages can include Heimdall load attributes for sections that should hydrate from the live ASP.NET Core app.")
    ];

    public IReadOnlyList<RouteExample> RouteExamples { get; } =
    [
        new("/", "SSG overview", "Fluent HTML"),
        new("/mvc-view/", "MVC static rendering", "Razor partial"),
        new("/markdown/", "Markdown-authored docs", "Markdig"),
        new("/hybrid/", "Hybrid load pattern", "Static + Heimdall")
    ];

    public IReadOnlyList<Metric> HeroMetrics { get; } =
    [
        new("4", "rendering styles"),
        new("5", "generated pages"),
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

    public RuntimeReferenceModel RuntimeReference { get; } = new(
        "Ready",
        [
            new("200", "content endpoint"),
            new("CSRF", "token flow"),
            new("inner", "swap mode"),
            new("RCL", "runtime asset")
        ],
        [
            "HeimdallHtml.OnLoad(\"hybrid.reference\") declares the load action.",
            "HeimdallHtml.Target(\"#runtime-reference-panel\") keeps the update scoped.",
            "The fallback remains useful when the site is exported to plain static hosting."
        ]);

    public IReadOnlyList<HybridNote> HybridNotes { get; } =
    [
        new("Generated shell", "The route is written to wwwroot/hybrid/index.html during the SSG pass."),
        new("Live target", "The reference card declares a Heimdall load action, target, and swap mode through fluent attributes."),
        new("Progressive fallback", "Plain static hosting still shows documentation; the live panel appears when the app runtime is available.")
    ];

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
    public string HybridHref { get; init; } = "/hybrid/";

    public string HomeHref { get; init; } = "/";
}

public sealed record MvcDocsCheckpoint(string Title, string Description);

public sealed record RuntimeReferenceModel(
    string Status,
    IReadOnlyList<Metric> Metrics,
    IReadOnlyList<string> Actions);

public sealed record HybridNote(string Title, string Description);
