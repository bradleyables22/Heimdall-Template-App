using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Rendering.Shared
{
	public static class SeoFragment
	{
		public static IHtmlContent Default => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("name", "description");
				m.ContentAttr("A modern Heimdall-powered web application.");
			});

			f.Meta(m =>
			{
				m.Attr("name", "robots");
				m.ContentAttr("index, follow");
			});

			f.Meta(m =>
			{
				m.Attr("name", "author");
				m.ContentAttr("Heimdall");
			});
		});

		public static IHtmlContent Twitter => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("name", "twitter:card");
				m.ContentAttr("summary_large_image");
			});

			f.Meta(m =>
			{
				m.Attr("name", "twitter:title");
				m.ContentAttr("Heimdall Web App");
			});

			f.Meta(m =>
			{
				m.Attr("name", "twitter:description");
				m.ContentAttr("A modern Heimdall-powered web application.");
			});

			f.Meta(m =>
			{
				m.Attr("name", "twitter:image");
				m.ContentAttr("/images/favicon.png");
			});
		});

		public static IHtmlContent OpenGraph => FluentHtml.Fragment(f =>
		{
			f.Meta(m =>
			{
				m.Attr("property", "og:type");
				m.ContentAttr("website");
			});

			f.Meta(m =>
			{
				m.Attr("property", "og:site_name");
				m.ContentAttr("Heimdall Web App");
			});

			f.Meta(m =>
			{
				m.Attr("property", "og:title");
				m.ContentAttr("Heimdall Web App");
			});

			f.Meta(m =>
			{
				m.Attr("property", "og:description");
				m.ContentAttr("A modern Heimdall-powered web application.");
			});

			f.Meta(m =>
			{
				m.Attr("property", "og:image");
				m.ContentAttr("/images/favicon.png");
			});
		});
	}
}
