using HeimdallTemplateApp.Rendering.Utilities;
using Microsoft.AspNetCore.Html;
using Heimdall.Server.Rendering;

namespace HeimdallTemplateApp.Rendering.Shared
{
	public static class MenuComponent
	{
		public static IHtmlContent Render(HttpContext ctx)
			=> FluentHtml.Fragment(f =>
			{
				// Top bar
				f.Div(bar =>
				{
					bar.Class(
						Bootstrap.Spacing.P(2),
						Bootstrap.Flex.AlignItemsCenter,
						Bootstrap.Display.Flex,
						Bootstrap.Flex.AlignContentBetween,
						Bootstrap.Bg.BgColor(Bootstrap.Color.Primary)
					);

					bar.Div(inner =>
					{
						inner.Button(btn =>
						{
							btn.Class(Bootstrap.Btn.Base, Bootstrap.Btn.Primary, Bootstrap.Text.Uppercase);
							btn.Attr("data-bs-toggle", "offcanvas");
							btn.Attr("data-bs-target", "#leftMenu");
							btn.Aria("controls", "leftMenu");

							btn.Tag("i", i =>
							{
								i.Class("bi", "bi-grid");
								i.Class(Bootstrap.Spacing.M(1, Bootstrap.Side.End));
							});

							btn.Text("Menu");
						});
					});
				});

				// Offcanvas
				f.Div(off =>
				{
					off.Class(Bootstrap.Offcanvas.Base, Bootstrap.Offcanvas.Start);
					off.Attr("tabindex", "-1");
					off.Id("leftMenu");
					off.Aria("labelledby", "leftMenuLabel");

					// Header
					off.Div(header =>
					{
						header.Class(Bootstrap.Offcanvas.Header);

						header.H5(h5 =>
						{
							h5.Class(Bootstrap.Offcanvas.Title);
							h5.Id("leftMenuLabel");

							h5.Tag("i", i => i.Class("bi", "bi-grid"));
							h5.Text(" Navigation");
						});

						header.Button(close =>
						{
							close.Type("button");
							close.Class("btn-close");
							close.Attr("data-bs-dismiss", "offcanvas");
							close.Aria("label", "Close");
						});
					});

					// Body
					off.Div(body =>
					{
						body.Class(Bootstrap.Offcanvas.Body);

						body.Div(list =>
						{
							list.Class(Bootstrap.ListGroup.Base);
							list.Id("navList");

							list.Add(NavItem("bi-house", "Home", "/"));
							list.Add(NavItem("bi-chat-dots", "Out Of Band", "/out-of-band"));
							list.Add(NavItem("bi-bezier2", "State", "/state"));
							list.Add(NavItem("bi-journal", "Forms", "/forms"));
							list.Add(NavItem("bi-arrow-repeat", "Lazy Loading", "/lazy"));
						});
					});
				});

				f.Script(s => s.Src("components/js/menu-component.js"));
			});

		private static IHtmlContent NavItem(string icon, string text, string href)
			=> FluentHtml.A(a =>
			{
				a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
				a.Href(href);

				a.Tag("i", i => i.Class("bi", icon, Bootstrap.Spacing.Me(2)));
				a.Text($" {text}");
			});
	}
}