using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Rendering.Shared;

namespace Server.Rendering.Pages
{
    public static class MainPage
    {
        [ContentInvocation]
        public static async Task<IHtmlContent> Render(Bifrost _bifrost)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            await _bifrost.PublishAsync(
                topic: "toasts",
                content: ToastManager.Create(new Toast
                {
                    Header = "Main Page Loading",
                    Content = "The main page is loading.",
                    Type = ToastType.Info,
                    DurationMs = 1500
                }),
                ttl: TimeSpan.FromMinutes(1)
            );

            var pageLoadedToast = ToastManager.Create(new Toast
            {
                Header = "Page Loaded",
                Content = "Main page finished loading successfully.",
                Type = ToastType.Success,
                DurationMs = 2500
            });

            await Task.Delay(TimeSpan.FromSeconds(5));

            return new HtmlString($@"
                <h1>Welcome to the Main Page</h1>
                <p>This is the main content of the page.</p>

                <invocation heimdall-content-target=""#toast-manager""
                            heimdall-content-swap=""afterbegin"">
                  {pageLoadedToast}
                </invocation>
                ");
        }
    }
}
