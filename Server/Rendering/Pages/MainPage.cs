using Microsoft.AspNetCore.Html;
using Server.Rendering.Shared;
using Server.Utilities;

namespace Server.Rendering.Pages
{
	public static class MainPage
	{
		public static IHtmlContent Render()
		{
			var hero = FluentHtml.Div(card =>
			{
				card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

				card.Div(body =>
				{
					body.Class(Bootstrap.Card.Body);
					body.Add(HeaderRow);
					body.Hr(_ => { });
					body.Add(Features);
				});
			});

			// Page frame
			return FluentHtml.Fragment(frame =>
			{
				frame.Div(row =>
				{
					row.Class(Bootstrap.Layout.Row, Bootstrap.Spacing.Mt(3));

					row.Div(_ => { _.Class(Bootstrap.Layout.Col); });

					row.Div(center =>
					{
						center.Class(Bootstrap.Layout.ColSpan(8, Bootstrap.Breakpoint.Lg));
						center.Add(hero);
					});

					row.Div(_ => { _.Class(Bootstrap.Layout.Col); });
				});
			});
		}

		private static IHtmlContent Features
			=> FluentHtml.Div(row =>
			{
				row.Class(
					Bootstrap.Layout.Row,
					Bootstrap.Layout.Gutter(3)
				);

				row.Add(
					FeatureCard.Render(
						icon: "bi bi-lightning-charge-fill",
						title: "Server actions",
						body: "Trigger backend methods with simple attributes and swap the returned HTML into the DOM."
					),
					FeatureCard.Render(
						icon: "bi bi-arrow-left-right",
						title: "Swap modes",
						body: "inner, outer, afterbegin, beforeend… keep updates tight and fast with predictable DOM changes."
					),
					FeatureCard.Render(
						icon: "bi bi-broadcast-pin",
						title: "Optional SSE (Bifrost)",
						body: "Push HTML updates to subscribed clients with topic-based streams—no websockets required."
					)
				);
			});

		private static IHtmlContent HeaderRow
			=> FluentHtml.Div(row =>
			{
				row.Class(
					Bootstrap.Display.Flex,
					Bootstrap.Flex.AlignItemsCenter,
					Bootstrap.Flex.JustifyBetween,
					Bootstrap.Flex.Wrap,
					Bootstrap.Spacing.Gap(3)
				);

				row.Div(left =>
				{
					left.Class(Bootstrap.Text.Start);

					left.H1(h =>
					{
						h.Class(
							Bootstrap.Raw("display-6"),
							Bootstrap.Spacing.Mb(2)
						);

						h.I(i =>
						{
							i.Class(
								Bootstrap.Raw("bi bi-shield-lock-fill"),
								Bootstrap.Spacing.Me(2)
							);
						});

						h.Text("Welcome to Heimdall");
					});

					left.P(p =>
					{
						p.Class(
							Bootstrap.Text.BodySecondary,
							Bootstrap.Spacing.Mb(0)
						);

						p.Text("HTML-first server actions + safe DOM swapping + optional SSE. ");
						p.Text("Build fast pages without a heavy client framework.");
					});
				});
			});
	}
}
