using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Rendering.Shared;
using Heimdall.Bootstrap;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Layouts
{
	public static class MainLayout
	{
		public static IHtmlContent Render(HttpContext ctx, IHtmlContent page, string title, bool enableSSE = true)
			=> FluentHtml.Fragment(f =>
			{
				f.Raw("<!DOCTYPE html>")
				.HtmlTag(html =>
				{
					html.Attr("lang", "en")
					.Head(head =>
					{
						head.Meta(m => m.Attr("charset", "utf-8"))
						.Meta(m =>
						{
							m.Name("viewport")
							.ContentAttr("width=device-width, initial-scale=1");
						})
						.Title(t => t.Text(title))
						.Add(
							SeoFragment.Twitter,
							SeoFragment.OpenGraph,
							SeoFragment.Default
						)
						.Link(l =>
						{
							l.Attr("rel", "icon")
							.Type("image/png")
							.Href("images/favicon.png");
						})
						.Link(l =>
						{
							l.Attr("rel", "stylesheet")
							.Href("css/app.css");
						})
						.Link(l =>
						{
							l.Attr("rel", "stylesheet")
							.Href("css/bootstrap.css");
						})
						.Link(l =>
						{
							l.Attr("rel", "stylesheet")
							.Href("css/bootstrap-icons.css");
						})
						.Script(s => s.Src("/_content/HeimdallFramework.Web/heimdall.js"))
						.Script(s => s.Src("js/bootstrap-bundle.js"));

						if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() == "development") 
							head.Script(s => s.Src("js/heimdall.debug.js"));
                    })
					.Body(body =>
					{
						body.Add(
							ToastManager.Render(ctx, enableSSE),
							MenuComponent.Render(ctx)
						)
						.Div(d =>
						{
							d.Class(Bootstrap.Layout.ContainerFluid, Bootstrap.Spacing.Py(2));
							d.Add(page);
						});
					});
				});
			});
	}
}
