using Microsoft.AspNetCore.Html;
using Server.Utilities;

namespace Server.Rendering.Shared
{
    public static class SeoFragment
    {
        public static IHtmlContent Default => Html.Fragment(
            Html.Meta(
                Html.Attr("name", "description"),
                Html.Attr("content", "A modern Heimdall-powered web application.")
            ),
            Html.Meta(
                Html.Attr("name", "robots"),
                Html.Attr("content", "index, follow")
            ),
            Html.Meta(
                Html.Attr("name", "author"),
                Html.Attr("content", "Heimdall")
            )
        );

        public static IHtmlContent Twitter => Html.Fragment(
            Html.Meta(
                Html.Attr("name", "twitter:card"),
                Html.Attr("content", "summary_large_image")
            ),
            Html.Meta(
                Html.Attr("name", "twitter:title"),
                Html.Attr("content", "Heimdall Web App")
            ),
            Html.Meta(
                Html.Attr("name", "twitter:description"),
                Html.Attr("content", "A modern Heimdall-powered web application.")
            ),
            Html.Meta(
                Html.Attr("name", "twitter:image"),
                Html.Attr("content", "/images/favicon.png")
            )
        );

        public static IHtmlContent OpenGraph => Html.Fragment(
            Html.Meta(
                Html.Attr("property", "og:type"),
                Html.Attr("content", "website")
            ),
            Html.Meta(
                Html.Attr("property", "og:site_name"),
                Html.Attr("content", "Heimdall Web App")
            ),
            Html.Meta(
                Html.Attr("property", "og:title"),
                Html.Attr("content", "Heimdall Web App")
            ),
            Html.Meta(
                Html.Attr("property", "og:description"),
                Html.Attr("content", "A modern Heimdall-powered web application.")
            ),
            Html.Meta(
                Html.Attr("property", "og:image"),
                Html.Attr("content", "/images/favicon.png")
            )
        );
    }
}
