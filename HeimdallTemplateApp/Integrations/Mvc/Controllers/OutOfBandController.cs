using Heimdall.Server;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using HeimdallTemplateApp.Infrastructure;
using HeimdallTemplateApp.Models;

namespace HeimdallTemplateApp.Controllers;

[ContentInvocationPrefix("toast")]
public sealed class OutOfBandController(
	Bifrost bifrost,
	IHeimdallMvcRenderer renderer) : Controller
{
	[Route("out-of-band")]
	public IActionResult Index()
	{
		ViewData["Title"] = "Out Of Band";
		return View();
	}

	[ContentInvocation("sse")]
	[RequestTimeout(3000)]
	public async Task<IHtmlContent> Sse(HttpContext ctx)
	{
		var toastHtml = await renderer.PartialAsync(
			"~/Views/Shared/_Toast.cshtml",
			new ToastItem
			{
				Header = "Hello from SSE!",
				Content = "This toast was published to your private toast topic and delivered over EventSource.",
				Type = ToastType.Success,
				DurationMs = 1800
			});

		await bifrost.PublishAsync(ctx.GetUserToastTopic(), toastHtml, TimeSpan.FromSeconds(10));
		return new HtmlString(string.Empty);
	}

	[ContentInvocation("oob")]
	[RequestTimeout(3000)]
	public async Task<IHtmlContent> Oob()
	{
		var toastHtml = await renderer.PartialAsync(
			"~/Views/Shared/_Toast.cshtml",
			new ToastItem
			{
				Header = "Hello from OOB!",
				Content = "This toast came back as an invocation targeting #toast-manager.",
				Type = ToastType.Success,
				DurationMs = 1800
			});

		return HeimdallHtml.Invocation(
			targetSelector: "#toast-manager",
			swap: HeimdallHtml.Swap.AfterBegin,
			payload: toastHtml);
	}
}
