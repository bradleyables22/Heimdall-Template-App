using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Rendering.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery();
builder.Services.AddCors();

builder.Services.AddHeimdall(options => 
    options.EnableDetailedErrors = true
);

var app = builder.Build();

app.UseAntiforgery();
app.UseCors();

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseStaticFiles();

app.UseHeimdall();

var commonComponents = new Dictionary<string, Func<IServiceProvider, HttpContext, IHtmlContent>>()
{
    ["{{menu}}"] = (sp, ctx) => MenuComponent.Render(ctx, "/"),
    ["{{toast-manager}}"] = (sp, ctx) => ToastManager.Render(ctx,true)

};

app.MapHeimdallPage(settings =>
{
    settings.Pattern = "/";
    settings.PagePath = "pages/main.html";
    settings.LayoutPath = "layouts/mainlayout.html";
    settings.LayoutPlaceholder = "{{page}}";
    settings.LayoutComponents = commonComponents;
});

app.Run();

