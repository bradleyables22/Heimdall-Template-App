using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("status")]
public sealed class IndexModel(IHeimdallMvcRenderer renderer) : PageModel
{
    [ContentInvocation("refresh")]
    public Task<IHtmlContent> Refresh(CancellationToken cancellationToken)
        => renderer.PartialAsync(
            "~/Pages/Shared/_StatusPanel.cshtml",
            new StatusPanelModel(DateTimeOffset.Now),
            cancellationToken);

    public void OnGet()
    {
    }

    public sealed record StatusPanelModel(DateTimeOffset? RefreshedAt = null);
}
