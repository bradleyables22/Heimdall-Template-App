using Microsoft.AspNetCore.Html;
using Server.Heimdall;
using Server.Utilities;
namespace Server.Rendering.Shared
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
    public class ToastItem
    {
        public string Header { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public ToastType Type { get; set; } = ToastType.Info;
        public int DurationMs { get; set; } = 3000;
    }

    public static class ToastManager
    {

        public static IHtmlContent Render(HttpContext ctx, bool useSSE = true)
        {
            IHtmlContent toastManager = Html.Div(
                    Html.Class(Bootstrap.Toast.Container, Bootstrap.Position.Fixed, Bootstrap.Position.Top0, Bootstrap.Position.End0, Bootstrap.Spacing.P(3)),
                    Html.Style("z-index:1000"),
                    Html.Id("toast-manager"),
                    Html.Aria("live", "polite"),
                    Html.Aria("atomic", "true"),

                    Html.When(useSSE,
                        HeimdallHtml.SseTopic("toasts"),
                        HeimdallHtml.SseTarget("#toast-manager"),
                        HeimdallHtml.SseSwapMode(HeimdallHtml.Swap.BeforeEnd)
                    ),
                    Html.Script(Html.Src("components/js/toast-manager.js"))
                );

            return toastManager;
        }


        public static IHtmlContent Create(ToastItem toast)
        {
            var toastClass = toast.GetToastClass();

            return Html.Div(
                Html.Class(Bootstrap.Toast.ToastBase, Bootstrap.Helpers.Fade, toastClass),
                Html.Role("alert"),
                Html.Aria("live", "assertive"),
                Html.Aria("atomic", "true"),
                Html.Data("bs-autohide", "true"),
                Html.Data("bs-delay", toast.DurationMs.ToString()),

                Html.Div(
                    Html.Class(Bootstrap.Toast.Header, toastClass, Bootstrap.Border.None),

                    Html.Tag("strong",
                        Html.Class(Bootstrap.Spacing.MeAuto()),
                        Html.Text(toast.Header)
                    ),

                    Html.Button(
                        Html.Class(Bootstrap.Btn.Close, Bootstrap.Spacing.Mb(1),Bootstrap.Spacing.Ms(2)),
                        Html.Data("bs-dismiss", "toast"),
                        Html.Aria("label", "Close")
                    )
                ),

                Html.Div(
                    Html.Class(Bootstrap.Toast.Body),
                    Html.Text(toast.Content)
                )
            );
        }

        static string GetToastClass(this ToastItem toast)
        {
            return toast.Type switch
            {
                ToastType.Success => Bootstrap.Bg.TextBg(Bootstrap.Color.Success),
                ToastType.Error => Bootstrap.Bg.TextBg(Bootstrap.Color.Danger),
                ToastType.Warning => Bootstrap.Bg.TextBg(Bootstrap.Color.Warning),
                ToastType.Info => Bootstrap.Bg.TextBg(Bootstrap.Color.Info),
                _ => Bootstrap.Bg.TextBg(Bootstrap.Color.Secondary),
            };
        }
    }
}
