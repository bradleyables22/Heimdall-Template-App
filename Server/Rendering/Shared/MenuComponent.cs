using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Scriban;

namespace Server.Rendering.Shared
{
    public static class MenuComponent
    {

        public static IHtmlContent Render(HttpContext ctx, string? pathName)
        {
            var path = pathName?.ToLowerInvariant() ?? "/";

            var active = path switch
            {
                "/" => "home",
                var p when p.StartsWith("/dashboard") => "dashboard",
                var p when p.StartsWith("/settings") => "settings",
                _ => null
            };

            var templatePath = Path.Combine(
                ctx.RequestServices
                   .GetRequiredService<IWebHostEnvironment>()
                   .WebRootPath,
                "components",
                "menu-component.template.html"
            );

            var templateText = File.ReadAllText(templatePath);

            var template = Template.Parse(templateText);

            if (template.HasErrors)
                throw new InvalidOperationException(
                    string.Join(
                        Environment.NewLine,
                        template.Messages.Select(m => m.Message)
                    )
                );

            var html = template.Render(new
            {
                active,
                path
            });

            return new HtmlString(html);
        }

    }
}
