using System.ComponentModel.DataAnnotations;
using Heimdall.Server;
using Heimdall.Server.Rendering;
using HeimdallTemplateApp.Models;
using HeimdallTemplateApp.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HeimdallTemplateApp.Pages;

[ContentInvocationPrefix("notes")]
public sealed class FormsModel(
    NoteStore notes,
    IHeimdallMvcRenderer renderer) : PageModel
{
    public NotesPageViewModel PageData { get; private set; } = new();

    [ContentInvocation("create")]
    public async Task<IHtmlContent> Create(
        [ContentPayload] CreateNoteRequest? noteRequest,
        CancellationToken cancellationToken)
    {
        noteRequest = Normalize(noteRequest ?? new CreateNoteRequest());
        var errors = Validate(noteRequest);

        if (errors.Count > 0)
        {
            return await renderer.PartialAsync(
                "~/Pages/Shared/_CreateNoteForm.cshtml",
                BuildFormModel(noteRequest, showErrors: true, errors),
                cancellationToken);
        }

        notes.Add(noteRequest);

        var formHtml = await renderer.PartialAsync(
            "~/Pages/Shared/_CreateNoteForm.cshtml",
            BuildFormModel(new CreateNoteRequest(), showErrors: false),
            cancellationToken);

        var notesHtml = await renderer.PartialAsync(
            "~/Pages/Shared/_NotesList.cshtml",
            notes.GetAll(),
            cancellationToken);

        var toastHtml = await renderer.PartialAsync(
            "~/Pages/Shared/_Toast.cshtml",
            new ToastItem
            {
                Header = "Note Created",
                Content = "A new note was successfully created",
                Type = ToastType.Success
            },
            cancellationToken);

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

    public void OnGet()
    {
        PageData = new NotesPageViewModel
        {
            Form = BuildFormModel(new CreateNoteRequest(), showErrors: false),
            Notes = notes.GetAll()
        };
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
                errors.TryAdd(memberName, result.ErrorMessage);
        }

        return errors;
    }
}
