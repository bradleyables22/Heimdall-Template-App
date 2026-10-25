using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace HeimdallTemplateApp.Services;

public sealed class StaticMvcRenderer(
    IHttpContextAccessor httpContextAccessor,
    IHeimdallMvcRenderer mvcRenderer)
{
    public async Task<IHtmlContent> PartialAsync(
        HeimdallStaticPageContext pageContext,
        string viewName,
        object? model)
    {
        var previousContext = httpContextAccessor.HttpContext;
        var httpContext = new DefaultHttpContext
        {
            RequestServices = pageContext.Services
        };
        httpContext.Request.Path = pageContext.Route;
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("example.com");

        httpContextAccessor.HttpContext = httpContext;

        try
        {
            return await mvcRenderer.PartialAsync(viewName, model, pageContext.CancellationToken);
        }
        finally
        {
            httpContextAccessor.HttpContext = previousContext;
        }
    }
}
