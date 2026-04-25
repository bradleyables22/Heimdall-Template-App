using Heimdall.Bootstrap;
using Microsoft.AspNetCore.Html;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Shared
{
	public static class FeatureCard
	{
		public static IHtmlContent Render(string icon, string title, string body)
			=> FluentHtml.Div(col =>
			{
				col.Class(
					Bootstrap.Layout.ColSpan(12),
					Bootstrap.Layout.ColSpan(4, Bootstrap.Breakpoint.Lg)
				)
				.Div(card =>
				{
					card.Class(
						Bootstrap.Border.Default,
						Bootstrap.Border.Rounded,
						Bootstrap.Spacing.P(3),
						Bootstrap.Sizing.H100
					)
					.Div(row =>
					{
						row.Class(
							Bootstrap.Display.Flex,
							Bootstrap.Spacing.Gap(2)
						)
						.I(i =>
						{
							i.Class(
								Bootstrap.Raw(icon),
								Bootstrap.Raw("fs-4")
							);
						})
						.Div(text =>
						{
							text.H4(h =>
							{
								h.Class(
									Bootstrap.Raw("h6"),
									Bootstrap.Spacing.Mb(1)
								)
								.Text(title);
							})
							.P(p =>
							{
								p.Class(
									Bootstrap.Spacing.Mb(0),
									Bootstrap.Text.BodySecondary
								).Text(body);
							});
						});
					});
				});
			});
	}
}
