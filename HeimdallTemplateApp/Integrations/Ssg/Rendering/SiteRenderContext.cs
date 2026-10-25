using HeimdallTemplateApp.Services;

namespace HeimdallTemplateApp.Rendering;

public sealed record SiteRenderContext(
    SiteContent Content,
    Func<string, string> ToSitePath);
