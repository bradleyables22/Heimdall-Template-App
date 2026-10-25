using Heimdall.Server;
using Heimdall.Server.Rendering;
using HeimdallTemplateApp.Infrastructure;
using HeimdallTemplateApp.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("toast")]
public sealed class OutOfBandModel(
    Bifrost bifrost,
    IHeimdallMvcRenderer renderer) : PageModel
{
    [ContentInvocation("sse")]
    [RequestTimeout(3000)]
    public async Task<IHtmlContent> Sse(CancellationToken cancellationToken)
    {
        var toastHtml = await renderer.PartialAsync(
            "~/Pages/Shared/_Toast.cshtml",
            new ToastItem
            {
                Header = "Hello from SSE!",
                Content = "This toast was published to your private toast topic and delivered over EventSource.",
                Type = ToastType.Success,
                DurationMs = 1800
            },
            cancellationToken);

        await bifrost.PublishAsync(HttpContext.GetUserToastTopic(), toastHtml, TimeSpan.FromSeconds(10));
        return HtmlString.Empty;
    }

    [ContentInvocation("oob")]
    [RequestTimeout(3000)]
    public async Task<IHtmlContent> Oob(CancellationToken cancellationToken)
    {
        var toastHtml = await renderer.PartialAsync(
            "~/Pages/Shared/_Toast.cshtml",
            new ToastItem
            {
                Header = "Hello from OOB!",
                Content = "This toast came back as an invocation targeting #toast-manager.",
                Type = ToastType.Success,
                DurationMs = 1800
            },
            cancellationToken);

        return HeimdallHtml.Invocation(
            targetSelector: "#toast-manager",
            swap: HeimdallHtml.Swap.AfterBegin,
            payload: toastHtml);
    }
}
