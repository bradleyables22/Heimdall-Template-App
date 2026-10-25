using Heimdall.Server;
using Heimdall.Server.Rendering;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using HeimdallTemplateApp.Models;
using HeimdallTemplateApp.Services;
using System.ComponentModel.DataAnnotations;

namespace HeimdallTemplateApp.Controllers;

[ContentInvocationPrefix("notes")]
public sealed class FormsController(
	NoteStore notes,
	IHeimdallMvcRenderer renderer) : Controller
{
	[Route("forms")]
	public IActionResult Index()
	{
		ViewData["Title"] = "Forms";

		return View(new NotesPageViewModel
		{
			Form = BuildFormModel(new CreateNoteRequest(), showErrors: false),
			Notes = notes.GetAll()
		});
	}

	[ContentInvocation("create")]
	public async Task<IHtmlContent> Create([ContentPayload] CreateNoteRequest? noteRequest)
	{
		noteRequest = Normalize(noteRequest ?? new CreateNoteRequest());
		var errors = Validate(noteRequest);

		if (errors.Count > 0)
		{
			return await renderer.PartialAsync(
				"~/Views/Forms/_CreateNoteForm.cshtml",
				BuildFormModel(noteRequest, showErrors: true, errors));
		}

		notes.Add(noteRequest);

		var formHtml = await renderer.PartialAsync(
			"~/Views/Forms/_CreateNoteForm.cshtml",
			BuildFormModel(new CreateNoteRequest(), showErrors: false));

		var notesHtml = await renderer.PartialAsync(
			"~/Views/Forms/_NotesList.cshtml",
			notes.GetAll());

		var toastHtml = await renderer.PartialAsync(
			"~/Views/Shared/_Toast.cshtml",
			new ToastItem
			{
				Header = "Note Created",
				Content = "A new note was successfully created",
				Type = ToastType.Success
			});

		var response = new HtmlContentBuilder();
		response.AppendHtml(formHtml);
		response.AppendHtml(HeimdallHtml.Invocation(
			targetSelector: "#notes-host",
			swap: HeimdallHtml.Swap.Inner,
			payload: notesHtml));
		response.AppendHtml(HeimdallHtml.Invocation(
			targetSelector: "#toast-manager",
			swap: HeimdallHtml.Swap.AfterBegin,
			payload: toastHtml));

		return response;
	}

	private static CreateNoteRequest Normalize(CreateNoteRequest request)
	{
		request.Title = (request.Title ?? string.Empty).Trim();
		request.Content = (request.Content ?? string.Empty).Trim();
		return request;
	}

	private static CreateNoteFormViewModel BuildFormModel(
		CreateNoteRequest request,
		bool showErrors,
		IReadOnlyDictionary<string, string>? errors = null) =>
		new()
		{
			Request = request,
			ShowErrors = showErrors,
			Errors = errors ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		};

	private static IReadOnlyDictionary<string, string> Validate(CreateNoteRequest request)
	{
		var results = new List<ValidationResult>();
		var context = new ValidationContext(request);
		Validator.TryValidateObject(request, context, results, validateAllProperties: true);

		var errors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		foreach (var result in results)
		{
			if (string.IsNullOrWhiteSpace(result.ErrorMessage))
				continue;

			foreach (var memberName in result.MemberNames)
			{
				errors.TryAdd(memberName, result.ErrorMessage);
			}
		}

		return errors;
	}
}
