using Microsoft.AspNetCore.Html;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Shared
{
	public static class SeoFragment
	{
		public static IHtmlContent Default => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("name", "description")
				.ContentAttr("A modern Heimdall-powered web application.");
			})
			.Meta(m =>
			{
				m.Attr("name", "robots")
				.ContentAttr("index, follow");
			})
			.Meta(m =>
			{
				m.Attr("name", "author")
				.ContentAttr("Heimdall");
			});
		});

		public static IHtmlContent Twitter => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("name", "twitter:card");
				m.ContentAttr("summary_large_image");
			})
			.Meta(m =>
			{
				m.Attr("name", "twitter:title")
				.ContentAttr("Heimdall Web App");
			})
			.Meta(m =>
			{
				m.Attr("name", "twitter:description")
				.ContentAttr("A modern Heimdall-powered web application.");
			})
			.Meta(m =>
			{
				m.Attr("name", "twitter:image")
				.ContentAttr("/images/favicon.png");
			});
		});

		public static IHtmlContent OpenGraph => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("property", "og:type")
				.ContentAttr("website");
			})
			.Meta(m =>
			{
				m.Attr("property", "og:site_name")
				.ContentAttr("Heimdall Web App");
			})
			.Meta(m =>
			{
				m.Attr("property", "og:title")
				.ContentAttr("Heimdall Web App");
			})
			.Meta(m =>
			{
				m.Attr("property", "og:description")
				.ContentAttr("A modern Heimdall-powered web application.");
			})
			.Meta(m =>
			{
				m.Attr("property", "og:image")
				.ContentAttr("/images/favicon.png");
			});
		});
	}
}
