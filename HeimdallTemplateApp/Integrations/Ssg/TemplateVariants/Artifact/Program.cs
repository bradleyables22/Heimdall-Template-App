using Heimdall.Server;
using HeimdallTemplateApp.Helpers;
using HeimdallTemplateApp.Rendering;
using HeimdallTemplateApp.Services;

var builder = WebApplication.CreateBuilder(args);
var siteBaseUrl = builder.Configuration["Site:BaseUrl"];
builder.WebHost.UseStaticWebAssets();

builder.Services.AddSingleton<SiteContent>();
builder.Services.AddSingleton<MarkdownPageRenderer>();
builder.Services.AddScoped<StaticMvcRenderer>();
builder.Services.AddHeimdallMvc();

var staticSite = builder.Services
    .AddHeimdallStaticSiteGeneration(options =>
    {
        options.UseContentRootPath("dist");
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

staticSite.WithNotFoundPage(
    ctx => RenderAssistant.RenderPage(ctx, "Not Found", "/404.html", NotFoundPage.Render));

var app = builder.Build();

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseDefaultFiles();
app.UseStaticFiles();

await app.RunWithHeimdallStaticSiteGenerationAsync(args);
