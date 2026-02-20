using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using HeimdallTemplateApp.Utilities;

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

							list.A(a =>
							{
								a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
								a.Href("/");

								a.Tag("i", i => i.Class("bi", "bi-house", Bootstrap.Spacing.Me(2)));
								a.Text(" Home");
							});

							list.A(a =>
							{
								a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
								a.Href("/out-of-band");

								a.Tag("i", i => i.Class("bi", "bi-chat-dots", Bootstrap.Spacing.Me(2)));
								a.Text(" Out Of Band");
							});

							list.A(a =>
							{
								a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
								a.Href("/state");

								a.Tag("i", i => i.Class("bi", "bi-bezier2", Bootstrap.Spacing.Me(2)));
								a.Text(" State");
							});
							list.A(a =>
							{
								a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
								a.Href("/forms");

								a.Tag("i", i => i.Class("bi", "bi-arrow-repeat", Bootstrap.Spacing.Me(2)));
								a.Text(" Forms");
							});

                            list.A(a =>
                            {
                                a.Class(Bootstrap.ListGroup.Item, Bootstrap.ListGroup.ItemAction);
                                a.Href("/lazy");

                                a.Tag("i", i => i.Class("bi", "bi-alarm", Bootstrap.Spacing.Me(2)));
                                a.Text(" Lazy Loading");
                            });
                        });
					});
				});

				f.Script(s => s.Src("components/js/menu-component.js"));
			});
	}
}
