using Microsoft.AspNetCore.Html;
using HeimdallTemplateApp.Utilities;

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
				);

				col.Div(card =>
				{
					card.Class(
						Bootstrap.Border.Default,
						Bootstrap.Border.Rounded,
						Bootstrap.Spacing.P(3),
						Bootstrap.Sizing.H100
					);

					card.Div(row =>
					{
						row.Class(
							Bootstrap.Display.Flex,
							Bootstrap.Spacing.Gap(2)
						);

						row.I(i =>
						{
							i.Class(
								Bootstrap.Raw(icon),
								Bootstrap.Raw("fs-4")
							);
						});

						row.Div(text =>
						{
							text.H4(h =>
							{
								h.Class(
									Bootstrap.Raw("h6"),
									Bootstrap.Spacing.Mb(1)
								);
								h.Text(title);
							});

							text.P(p =>
							{
								p.Class(
									Bootstrap.Spacing.Mb(0),
									Bootstrap.Text.BodySecondary
								);
								p.Text(body);
							});
						});
					});
				});
			});
	}
}
