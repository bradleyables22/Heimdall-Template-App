using Heimdall.Server;
using Heimdall.Server.Rendering;
using HeimdallTemplateApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NoteStore>();
builder.Services.AddSingleton<WeatherForecastStore>();

builder.Services
	.AddHeimdall(options =>
	{
		options.EnableDetailedErrors = builder.Environment.IsDevelopment();
	})
	.AddHeimdallMvc();

builder.Services.AddAntiforgery();

var app = builder.Build();

StaticAssets.Discover(app.Environment.WebRootPath);

app.UseAntiforgery();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseRouting();

app.UseHeimdall();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.Run();
