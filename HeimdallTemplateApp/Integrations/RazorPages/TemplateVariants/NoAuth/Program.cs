using Heimdall.Server;
using HeimdallTemplateApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<NoteStore>();
builder.Services.AddSingleton<WeatherForecastStore>();
builder.Services.AddHeimdallMvc();
builder.Services.AddAntiforgery();
builder.Services.AddHeimdall(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());

var app = builder.Build();

app.UseAntiforgery();
app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseHeimdall();

app.MapRazorPages();

app.Run();
