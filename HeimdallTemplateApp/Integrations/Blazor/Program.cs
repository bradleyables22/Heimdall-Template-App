using Heimdall.Server;
using HeimdallTemplateApp.Authentication;
using HeimdallTemplateApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHeimdallBlazor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAntiforgery();
builder.Services.AddHeimdallAuthentication(builder.Configuration);
builder.Services.AddHeimdall(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseHeimdall();
app.MapHeimdallAuthentication();

var componentEndpoints = app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (builder.Configuration.GetValue<bool>("Authentication:Oidc:Enabled"))
{
    componentEndpoints.RequireAuthorization();
}

app.Run();
