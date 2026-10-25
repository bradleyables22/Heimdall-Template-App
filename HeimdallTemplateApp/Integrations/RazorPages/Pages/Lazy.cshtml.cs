using Heimdall.Server;
using HeimdallTemplateApp.Models;
using HeimdallTemplateApp.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("weather")]
public sealed class LazyModel(
    WeatherForecastStore weather,
    IHeimdallMvcRenderer renderer) : PageModel
{
    private const int InitialTake = 10;

    public WeatherRowsViewModel Rows { get; private set; } = new();

    [ContentInvocation("loadMore")]
    [RequestTimeout(3000)]
    public Task<IHtmlContent> LoadMore(
        [ContentPayload] WeatherLoadMoreRequest? request,
        CancellationToken cancellationToken)
    {
        request ??= new WeatherLoadMoreRequest();

        if (request.Offset < 0)
            request.Offset = 0;

        if (request.Take <= 0)
            request.Take = InitialTake;

        var rows = weather.GetRows(request.Offset, request.Take);
        var nextOffset = request.Offset + rows.Count;

        return renderer.PartialAsync(
            "~/Pages/Shared/_WeatherRows.cshtml",
            new WeatherRowsViewModel
            {
                Rows = rows,
                Offset = nextOffset,
                Take = request.Take,
                TotalCount = weather.Count
            },
            cancellationToken);
    }

    public void OnGet()
    {
        Rows = new WeatherRowsViewModel
        {
            Rows = weather.GetRows(0, InitialTake),
            Offset = InitialTake,
            Take = InitialTake,
            TotalCount = weather.Count
        };
    }
}
