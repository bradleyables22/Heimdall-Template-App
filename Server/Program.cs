using Heimdall.Server;
using Server.Pages;
using Server.Rendering.Layouts;
using Server.Rendering.Pages;

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

app.MapHeimdallPage("/",(_, ctx) => 
{
    return MainLayout.Render(ctx, MainPage.Render(), "Home", true);
});
app.MapHeimdallPage("/out-of-band", (_, ctx) =>
{
    return MainLayout.Render(ctx, OobPage.Render(), "Out Of Band", true);
});
app.MapHeimdallPage("/state", (_, ctx) =>
{
    return MainLayout.Render(ctx, CounterPage.Render(), "State", true);
});
app.Run();

