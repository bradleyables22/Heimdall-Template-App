
using Heimdall.Server;
using Microsoft.AspNetCore.Html;

namespace Server.Rendering.Layouts
{
    public static class MainLayout
    {
        [ContentInvocation]
        public static IHtmlContent Clear() => new HtmlString(string.Empty);

    }
}
