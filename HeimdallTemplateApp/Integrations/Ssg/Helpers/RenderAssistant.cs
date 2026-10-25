using Heimdall.Server;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Rendering;
using HeimdallTemplateApp.Services;

namespace HeimdallTemplateApp.Helpers
{
	public static class RenderAssistant
	{
		public static IHtmlContent RenderPage( HeimdallStaticPageContext ctx, string title, string activePath, Func<SiteRenderContext, IHtmlContent> render)
		{
			var site = CreateSiteContext(ctx);
			return SiteLayout.Render(title, activePath, site.ToSitePath, render(site));
		}

		public static async Task<IHtmlContent> RenderMarkdownPageAsync(HeimdallStaticPageContext ctx)
		{
			var site = CreateSiteContext(ctx);
			var markdown = ctx.GetRequiredService<MarkdownPageRenderer>();
			var docs = await markdown.RenderAsync("Content/static-generation.md", ctx.CancellationToken);

			var body = FluentHtml.Fragment(page =>
			{
				page.Section(section =>
				{
					section.Class("py-5 border-bottom")
						.Div(container =>
						{
							container.Class("container")
								.Div(row =>
								{
									row.Class("row justify-content-center")
										.Div(col =>
										{
											col.Class("col-lg-9")
												.P(p => p.Class("eyebrow text-primary fw-semibold mb-2").Text("Markdown documentation page"))
												.H1(h => h.Class("display-5 fw-bold mb-3").Text("Author Docs In Markdown"))
												.P(p => p.Class("lead text-secondary mb-0").Text("This route is generated from a markdown file with Markdig, then wrapped in the same Bootstrap layout as the fluent and MVC examples."));
										});
								});
						});
				})
				.Section(section =>
				{
					section.Class("py-5")
						.Div(container =>
						{
							container.Class("container")
								.Div(row =>
								{
									row.Class("row justify-content-center")
										.Div(col =>
										{
											col.Class("col-lg-8 markdown-body")
												.Add(docs);
										});
								});
						});
				});
			});

			return SiteLayout.Render("Markdown Docs", "/markdown/", site.ToSitePath, body);
		}
		public static async Task<IHtmlContent> RenderMvcViewPageAsync(HeimdallStaticPageContext ctx)
		{
			var site = CreateSiteContext(ctx);
			var renderer = ctx.GetRequiredService<StaticMvcRenderer>();
			var view = await renderer.PartialAsync(
				ctx,
				"/Views/Shared/_MvcDocs.cshtml",
				site.Content.MvcDocs with
				{
					HomeHref = site.ToSitePath("/")
				});

			return SiteLayout.Render("MVC View Docs", "/mvc-view/", site.ToSitePath, view);
		}

		private static SiteRenderContext CreateSiteContext(HeimdallStaticPageContext ctx)
		{
			var content = ctx.GetRequiredService<SiteContent>();
			return new SiteRenderContext(content, ctx.ToSitePath);
		}
	}
}
