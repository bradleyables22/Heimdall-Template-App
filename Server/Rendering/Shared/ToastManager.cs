using Microsoft.AspNetCore.Html;
using System.Net;
using System.Xml;

namespace Server.Rendering.Shared
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }
    public class Toast
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
            var env = ctx.RequestServices.GetRequiredService<IWebHostEnvironment>();

            var filePath = Path.Combine(
                env.WebRootPath,
                "components",
                "toast-manager.html"
            );

            var html = File.ReadAllText(filePath);

            if (!useSSE)
            {
                html = html
                    .Replace(" heimdall-sse=\"toasts\"", "")
                    .Replace(" heimdall-sse-target=\"#toast-manager\"", "")
                    .Replace(" heimdall-sse-swap=\"afterbegin\"", "");
            }

            return new HtmlString(html);
        }


        public static IHtmlContent Create(Toast toast)
        {
            string toastClass = toast.GetToastClass();

            return new HtmlString(
                @$"<div class=""toast fade {toastClass}""
                      role=""alert""
                      aria-live=""assertive""
                      aria-atomic=""true""
                      data-bs-autohide=""true""
                      data-bs-delay=""{toast.DurationMs}"">
                      <div class=""toast-header {toastClass} border-0"">
                        <strong class=""me-auto"">{toast.Header}</strong>
                        <button type=""button""
                                class=""btn-close ms-2 mb-1""
                                data-bs-dismiss=""toast""
                                aria-label=""Close""></button>
                      </div>
                      <div class=""toast-body"">
                        {toast.Content}
                      </div>
                 </div>"
                );
        }

        static string GetToastClass(this Toast toast)
        {
            return toast.Type switch
            {
                ToastType.Success => "text-bg-success",
                ToastType.Error => "text-bg-danger",
                ToastType.Warning => "text-bg-warning",
                ToastType.Info => "text-bg-info",
                _ => "text-bg-secondary"
            };
        }
    }
}
