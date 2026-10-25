using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using HeimdallTemplateApp.Models;

namespace HeimdallTemplateApp.Controllers;

[ContentInvocationPrefix("counter")]
public sealed class StateController(IHeimdallMvcRenderer renderer) : Controller
{
	[Route("state")]
	public IActionResult Index()
	{
		ViewData["Title"] = "State";
		return View(new CounterState());
	}

	[ContentInvocation("inc")]
	[RequestTimeout(3000)]
	public Task<IHtmlContent> Increment([ContentPayload] CounterState? state) =>
		RenderCounterHostAsync((state?.Count ?? 0) + 1);

	[ContentInvocation("dec")]
	[RequestTimeout(3000)]
	public Task<IHtmlContent> Decrement([ContentPayload] CounterState? state) =>
		RenderCounterHostAsync((state?.Count ?? 0) - 1);

	[ContentInvocation("reset")]
	[RequestTimeout(3000)]
	public Task<IHtmlContent> Reset([ContentPayload] CounterState? state) =>
		RenderCounterHostAsync(0);

	private Task<IHtmlContent> RenderCounterHostAsync(int count) =>
		renderer.PartialAsync("~/Views/State/_CounterHost.cshtml", new CounterState { Count = count });
}
