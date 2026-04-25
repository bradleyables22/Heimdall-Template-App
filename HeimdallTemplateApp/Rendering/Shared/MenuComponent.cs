using Heimdall.Bootstrap;
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
					).Div(inner =>
					{
						inner.Button(btn =>
						{
							btn.Class(Bootstrap.Btn.Base, Bootstrap.Btn.Primary, Bootstrap.Text.Uppercase)
							.Attr("data-bs-toggle", "offcanvas")
							.Attr("data-bs-target", "#leftMenu")
							.Aria("controls", "leftMenu")
							.Tag("i", i =>
							{
								i.Class("bi", "bi-grid")
								.Class(Bootstrap.Spacing.M(1, Bootstrap.Side.End));
							})
							.Text("Menu");
						});
					});
				})
				.Div(off =>
				{
					off.Class(Bootstrap.Offcanvas.Base, Bootstrap.Offcanvas.Start)
					.Attr("tabindex", "-1")
					.Id("leftMenu")
					.Aria("labelledby", "leftMenuLabel")
					.Div(header =>
					{
						header.Class(Bootstrap.Offcanvas.Header)
						.H5(h5 =>
						{
							h5.Class(Bootstrap.Offcanvas.Title)
							.Id("leftMenuLabel")
							.Tag("i", i => i.Class("bi", "bi-grid"))
							.Text(" Navigation");
						})
						.Button(close =>
						{
							close.Type("button")
							.Class("btn-close")
							.Attr("data-bs-dismiss", "offcanvas")
							.Aria("label", "Close");
						});
					})
					.Div(body =>
					{
						body.Class(Bootstrap.Offcanvas.Body)
						.Div(list =>
						{
							list.Class(Bootstrap.ListGroup.Base)
							.Id("navList")
							.Add(NavItem("bi-house", "Home", "/"))
							.Add(NavItem("bi-chat-dots", "Out Of Band", "/out-of-band"))
							.Add(NavItem("bi-bezier2", "State", "/state"))
							.Add(NavItem("bi-journal", "Forms", "/forms"))
							.Add(NavItem("bi-arrow-repeat", "Lazy Loading", "/lazy"));
						});
					});
				})
				.Script(s => s.Src("components/js/menu-component.js"));
			});

		private static IHtmlContent NavItem(string icon, string text, string href)
			=> FluentHtml.A(a =>
			{
				a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction)
				.Href(href)
				.Tag("i", i => i.Class("bi", icon, Bootstrap.Spacing.Me(2)))
				.Text($" {text}");
			});
	}
}