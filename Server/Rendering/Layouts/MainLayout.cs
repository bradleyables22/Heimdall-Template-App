using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Rendering.Shared;
using HeimdallTemplateApp.Utilities;

namespace HeimdallTemplateApp.Rendering.Layouts
{
	public static class MainLayout
	{
		public static IHtmlContent Render(HttpContext ctx, IHtmlContent page, string title, bool enableSSE = true)
			=> FluentHtml.Fragment(f =>
			{
				f.Raw("<!DOCTYPE html>");

				f.HtmlTag(html =>
				{
					html.Attr("lang", "en");

					html.Head(head =>
					{
						head.Meta(m => m.Attr("charset", "utf-8"));

						head.Meta(m =>
						{
							m.Name("viewport");
							m.ContentAttr("width=device-width, initial-scale=1");
						});

						head.Title(t => t.Text(title));

						head.Add(
							SeoFragment.Twitter,
							SeoFragment.OpenGraph,
							SeoFragment.Default
						);

						head.Link(l =>
						{
							l.Attr("rel", "icon");
							l.Type("image/png");
							l.Href("images/favicon.png");
						});

						head.Link(l =>
						{
							l.Attr("rel", "stylesheet");
							l.Href("css/app.css");
						});

						head.Link(l =>
						{
							l.Attr("rel", "stylesheet");
							l.Href("css/bootstrap.css");
						});

						head.Link(l =>
						{
							l.Attr("rel", "stylesheet");
							l.Href("css/bootstrap-icons.css");
						});

						head.Script(s => s.Src("/_content/HeimdallFramework.Web/heimdall.js"));
						head.Script(s => s.Src("js/bootstrap-bundle.js"));
					});

					html.Body(body =>
					{
						body.Add(
							ToastManager.Render(ctx, enableSSE),
							MenuComponent.Render(ctx)
						);

						body.Div(d =>
						{
							d.Class(Bootstrap.Layout.ContainerFluid, Bootstrap.Spacing.Py(2));
							d.Add(page);
						});
					});
				});
			});
	}
}
