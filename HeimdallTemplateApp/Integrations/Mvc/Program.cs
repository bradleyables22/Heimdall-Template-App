using Heimdall.Server;
using Heimdall.Server.Rendering;
using HeimdallTemplateApp.Authentication;
using HeimdallTemplateApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHeimdallAuthentication(builder.Configuration);
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
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHeimdall();
app.MapHeimdallAuthentication();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}")
	.WithStaticAssets();

app.Run();
