using Heimdall.Server;
using HeimdallTemplateApp.Helpers;
using HeimdallTemplateApp.Rendering;
using HeimdallTemplateApp.Services;

var outputMode = "__HEIMDALL_SSG_OUTPUT_MODE__";
var builder = WebApplication.CreateBuilder(args);
var siteBaseUrl = builder.Configuration["Site:BaseUrl"];
builder.WebHost.UseStaticWebAssets();

builder.Services.AddSingleton<SiteContent>();
builder.Services.AddSingleton<MarkdownPageRenderer>();
builder.Services.AddScoped<StaticMvcRenderer>();
builder.Services.AddAntiforgery();
builder.Services.AddHeimdall(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});
builder.Services.AddHeimdallMvc();
var staticSite = builder.Services
    .AddHeimdallStaticSiteGeneration(options =>
    {
        if (outputMode == "artifact")
        {
            options.UseContentRootPath("dist");
        }
        else
        {
            options.UseWebRootPath();
        }

        options.CleanOutputPath = true;
        options.CopyWebRootAssets = true;
        options.CopyStaticWebAssets = true;

        if (Uri.TryCreate(siteBaseUrl, UriKind.Absolute, out var siteUri)
            && (siteUri.Scheme == Uri.UriSchemeHttp || siteUri.Scheme == Uri.UriSchemeHttps))
        {
            options.UseSitemap(siteBaseUrl);
        }

        options.UseRobotsTxt();
    })
    .WithStaticPage("/", ctx => RenderAssistant.RenderPage(ctx, "Heimdall SSG Docs", "/", HomePage.Render))
    .WithStaticPage("/mvc-view", RenderAssistant.RenderMvcViewPageAsync)
    .WithStaticPage("/markdown", RenderAssistant.RenderMarkdownPageAsync);

if (outputMode == "hybrid")
{
    staticSite.WithStaticPage(
        "/hybrid",
        ctx => RenderAssistant.RenderPage(ctx, "Hybrid Docs", "/hybrid/", HybridPage.Render));
}

staticSite.WithNotFoundPage(
    ctx => RenderAssistant.RenderPage(ctx, "Not Found", "/404.html", NotFoundPage.Render));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAntiforgery();

if (outputMode == "hybrid")
{
    app.UseHeimdall();
}

await app.RunWithHeimdallStaticSiteGenerationAsync(args);
