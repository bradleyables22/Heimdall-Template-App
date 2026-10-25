using Heimdall.Server;
using HeimdallTemplateApp.Authentication;
using HeimdallTemplateApp.Components;
using HeimdallTemplateApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);
builder.Services.AddRazorComponents();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<NoteStore>();
builder.Services.AddSingleton<WeatherForecastStore>();
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

var componentEndpoints = app.MapRazorComponents<App>();

if (builder.Configuration.GetValue<bool>("Authentication:Oidc:Enabled"))
{
    componentEndpoints.RequireAuthorization();
}

app.Run();
