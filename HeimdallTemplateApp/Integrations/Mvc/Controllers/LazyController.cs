using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using HeimdallTemplateApp.Models;
using HeimdallTemplateApp.Services;

namespace HeimdallTemplateApp.Controllers;

[ContentInvocationPrefix("weather")]
public sealed class LazyController(
	WeatherForecastStore weather,
	IHeimdallMvcRenderer renderer) : Controller
{
	private const int InitialTake = 10;

	[Route("lazy")]
	public IActionResult Index()
	{
		ViewData["Title"] = "Lazy Loading";

		return View(new WeatherRowsViewModel
		{
			Rows = weather.GetRows(0, InitialTake),
			Offset = InitialTake,
			Take = InitialTake,
			TotalCount = weather.Count
		});
	}

	[ContentInvocation("loadMore")]
	[RequestTimeout(3000)]
	public Task<IHtmlContent> LoadMore([ContentPayload] WeatherLoadMoreRequest? request)
	{
		request ??= new WeatherLoadMoreRequest();

		if (request.Offset < 0)
			request.Offset = 0;

		if (request.Take <= 0)
			request.Take = InitialTake;

		var rows = weather.GetRows(request.Offset, request.Take);
		var nextOffset = request.Offset + rows.Count;

		return renderer.PartialAsync(
			"~/Views/Lazy/_WeatherRows.cshtml",
			new WeatherRowsViewModel
			{
				Rows = rows,
				Offset = nextOffset,
				Take = request.Take,
				TotalCount = weather.Count
			});
	}
}
