using Heimdall.Server;
using HeimdallTemplateApp.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("counter")]
public sealed class StateModel(IHeimdallMvcRenderer renderer) : PageModel
{
    [ContentInvocation("inc")]
    [RequestTimeout(3000)]
    public Task<IHtmlContent> Increment([ContentPayload] CounterState? state, CancellationToken cancellationToken) =>
        RenderCounterHostAsync((state?.Count ?? 0) + 1, cancellationToken);

    [ContentInvocation("dec")]
    [RequestTimeout(3000)]
    public Task<IHtmlContent> Decrement([ContentPayload] CounterState? state, CancellationToken cancellationToken) =>
        RenderCounterHostAsync((state?.Count ?? 0) - 1, cancellationToken);

    [ContentInvocation("reset")]
    [RequestTimeout(3000)]
    public Task<IHtmlContent> Reset([ContentPayload] CounterState? state, CancellationToken cancellationToken) =>
        RenderCounterHostAsync(0, cancellationToken);

    public void OnGet()
    {
    }

    private Task<IHtmlContent> RenderCounterHostAsync(int count, CancellationToken cancellationToken) =>
        renderer.PartialAsync(
            "~/Pages/Shared/_CounterHost.cshtml",
            new CounterState { Count = count },
            cancellationToken);
}
