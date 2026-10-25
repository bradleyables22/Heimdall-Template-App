using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("main")]
public sealed class IndexModel(IHeimdallMvcRenderer renderer) : PageModel
{
    [ContentInvocation("body")]
    public Task<IHtmlContent> Body(CancellationToken cancellationToken)
        => renderer.PartialAsync(
            "~/Pages/Shared/_Body.cshtml",
            model: null,
            cancellationToken);

    public void OnGet()
    {
    }
}
