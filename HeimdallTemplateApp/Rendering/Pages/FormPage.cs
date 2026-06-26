using Heimdall.Bootstrap;
using Heimdall.Server;
using Heimdall.Server.Rendering;
using HeimdallTemplateApp.Rendering.Shared;
using Microsoft.AspNetCore.Html;     
using System.ComponentModel.DataAnnotations;
using static Heimdall.Bootstrap.Bootstrap;
using static Heimdall.Server.Rendering.Html;

namespace HeimdallTemplateApp.Rendering.Pages
{
    public static class FormPage
    {
        public const string CreateHostId = "create-note-host";
        public const string HostId = "notes-host";
        public const string FormId = "create-note-form";
        private const string TitleInputId = "title";
        private const string ContentInputId = "content";

        public static class Actions
        {
            public const string Create = "notes.create";
        }

		public static List<NoteEntity> Notes { get; set; } = new();

        public sealed class NoteEntity
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
            public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        }

        public sealed class CreateNoteRequest
        {
            [StringLength(125, MinimumLength = 5,
                ErrorMessage = "Title must be between 5 and 125 characters long.")]
            public string Title { get; set; } = string.Empty;

            [StringLength(500, MinimumLength = 5,
                ErrorMessage = "Content must be between 5 and 500 characters long.")]
            public string Content { get; set; } = string.Empty;
        }

        public static IHtmlContent Render()
        {
            // Initial render: don't show errors until submit.
            var request = new CreateNoteRequest();
            var results = ValidateInternal(Normalize(request));

            return FluentHtml.Div(row =>
            {
                row.Class(Bootstrap.Layout.Row, Bootstrap.Spacing.Mt(4))
                .Div(col1 =>
                {
                    col1.Class(Bootstrap.Layout.ColSpan(3, Bootstrap.Breakpoint.Lg));
                })
                .Div(col2 =>
                {
                    col2.Class(Bootstrap.Layout.ColSpan(6, Bootstrap.Breakpoint.Lg))
                    .Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg)
                        .Div(header =>
                        {
                            header.Class(Bootstrap.Card.Header)
                            .H2(h => h.Text("Create New Note"));
                        })
                        .Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body)
                            .Div(host =>
                            {
                                host.Id(CreateHostId)
                                .Add(GenerateForm(request, results));
                            });
                        });
                    }).Div(sp => sp.Class(Bootstrap.Spacing.Mt(4)))
                    .Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg)
                        .Div(header =>
                        {
                            header.Class(Bootstrap.Card.Header)
                            .H2(h => h.Text("Notes"));
                        })
                        .Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body)
                            .Div(host =>
                            {
                                host.Id(HostId)
                                .Add(RenderNotesList());
                            });
                        });
                    });
                })
                .Div(col3 =>
                {
                    col3.Class(Bootstrap.Layout.ColSpan(3, Bootstrap.Breakpoint.Lg));
                });
            });
        }

        internal static IHtmlContent GenerateForm(CreateNoteRequest req, List<ValidationResult> results, bool showErrors = false)
        {
            var titleError = FirstErrorFor(results, nameof(CreateNoteRequest.Title));
            var contentError = FirstErrorFor(results, nameof(CreateNoteRequest.Content));

            return FluentHtml.Form(form =>
            {
                form.Id(FormId);

                // Submit -> CreateNote
                // Target the host but swap INNER to avoid replacing the host node.
                form.Heimdall()
                    .Submit(Actions.Create)
                    .PayloadFromClosestForm()
                    .Target($"#{CreateHostId}")
                    .SwapInner()
                    .PreventDefault(true);

                form.Label(l =>
                {
                    l.Class(Bootstrap.Form.Label)
                    .For(TitleInputId)
                    .Text("Title");
                })
                .Input(InputType.text, input =>
                {
                    input.Id(TitleInputId)
                    .Name(nameof(CreateNoteRequest.Title))
                    .Value(req.Title ?? string.Empty)
                    .Attr("maxlength", "125");

                    input.Class(Bootstrap.Form.Control, Bootstrap.Spacing.Mb(1));
                });

                if (showErrors && titleError != null)
                {
                    form.Div(err =>
                    {
                        err.Class(Bootstrap.Form.Text, Bootstrap.Text.TxtColor(Bootstrap.Color.Danger), Bootstrap.Spacing.Mb(3))
                        .Text(titleError);
                    });
                }
                else
                {
                    form.Div(sp => sp.Class(Bootstrap.Spacing.Mb(3)));
                }
                form.Label(l =>
                {
                    l.Class(Bootstrap.Form.Label)
                    .For(ContentInputId)
                    .Text("Content");
                });

                // Assumes you added FluentHtml.TextArea(...) wrapper previously.
                // If not, you can replace with form.Tag("textarea", ...)
                form.TextArea(input =>
                {
                    input.Id(ContentInputId)
                    .Name(nameof(CreateNoteRequest.Content))
                    .Text(req.Content ?? string.Empty)
                    .Attr("maxlength", "500")
                    .Attr("rows", "5");

                    input.Class(Bootstrap.Form.Control, Bootstrap.Spacing.Mb(1));
                });

                if (showErrors && contentError != null)
                {
                    form.Div(err =>
                    {
                        err.Class(Bootstrap.Form.Text, Bootstrap.Text.TxtColor(Bootstrap.Color.Danger), Bootstrap.Spacing.Mb(3))
                        .Text(contentError);
                    });
                }
                else
                {
                    form.Div(sp => sp.Class(Bootstrap.Spacing.Mb(3)));
                }
                form.Div(r =>
                {
                    r.Class(Bootstrap.Layout.Row);

                    r.Div(full =>
                    {
                        full.Class(Bootstrap.Layout.ColSpan(12, Bootstrap.Breakpoint.Lg))
                        .Add(SubmitButton());
                    });
                });
            });
        }

        private static IHtmlContent SubmitButton()
        {
            return FluentHtml.Button(btn =>
            {
                btn.Class(Bootstrap.Btn.Primary, Bootstrap.Sizing.W100)
                .Attr("type", "submit");

                btn.Text("Submit");
            });
        }

        internal static IHtmlContent RenderNotesList()
        {
            if (Notes.Count == 0)
            {
                return FluentHtml.Div(d =>
                {
                    d.Class(Bootstrap.Text.BodySecondary)
                    .Text("No notes yet.");
                });
            }

            var ordered = Notes
                .OrderByDescending(x => x.CreatedUtc)
                .ToList();

            return FluentHtml.Div(list =>
            {
                list.Class(Bootstrap.Flex.VStack, Bootstrap.Spacing.Gap(2));

                foreach (var note in ordered)
                {
                    list.Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Sm);

                        card.Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body)
                            .H5(h =>
                            {
                                h.Class(Bootstrap.Card.Title)
                                .Text(note.Title);
                            })
                            .Div(s =>
                            {
                                s.Class(Bootstrap.Text.BodySecondary, Bootstrap.Text.Small, Bootstrap.Spacing.Mb(2))
                                .Text($"Created {note.CreatedUtc:yyyy-MM-dd HH:mm} UTC");
                            })
                            .Div(p =>
                            {
                                p.Class(Bootstrap.Card.Text)
                                .Text(note.Content);
                            });
                        });
                    });
                }
            });
        }


        internal static CreateNoteRequest Normalize(CreateNoteRequest req)
        {
            req.Title = (req.Title ?? string.Empty).Trim();
            req.Content = (req.Content ?? string.Empty).Trim();
            return req;
        }

        internal static List<ValidationResult> ValidateInternal(CreateNoteRequest req)
        {
            var context = new ValidationContext(req);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(req, context, results, validateAllProperties: true);
            return results;
        }

        private static string? FirstErrorFor(List<ValidationResult> results, string memberName)
        {
            if (results is null || results.Count == 0)
                return null;

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                if (r is null || r.MemberNames is null)
                    continue;

                foreach (var m in r.MemberNames)
                {
                    if (string.Equals(m, memberName, StringComparison.OrdinalIgnoreCase))
                        return r.ErrorMessage;
                }
            }

            return null;
        }

		[ContentInvocationPrefix("notes")]
		public static class NoteActions
		{
			// Called on submit. Re-validates, creates, clears, and OOB-updates notes list.
			[ContentInvocation("create")]
			public static IHtmlContent Create([ContentPayload] FormPage.CreateNoteRequest noteRequest)
			{
				noteRequest ??= new FormPage.CreateNoteRequest();

				noteRequest = FormPage.Normalize(noteRequest);
				var results = FormPage.ValidateInternal(noteRequest);
				var noteCreated = false;

				if (!results.Any())
				{
					FormPage.Notes.Add(new FormPage.NoteEntity
					{
						Title = noteRequest.Title,
						Content = noteRequest.Content
					});
					noteCreated = true;

					// Clear form after success, and don't show errors on empty.
					noteRequest = new FormPage.CreateNoteRequest();
					results = FormPage.ValidateInternal(FormPage.Normalize(noteRequest));
				}

				// Main swap updates the create-note host (inner swap). Successful submits also
				// push OOB updates for the notes host and toast manager.
				return FluentHtml.Fragment(f =>
				{
					// Main content for the target: just the form markup
					f.Add(FormPage.GenerateForm(noteRequest, results, showErrors: !noteCreated));

					if (!noteCreated)
						return;

					f.Heimdall().Invocation(
						targetSelector: $"#{FormPage.HostId}",
						swap: HeimdallHtml.Swap.Inner,
						payload: FormPage.RenderNotesList(),
						wrapInTemplate: false
					);

					var toast = new ToastItem
					{
						Header = "Note Created",
						Content = "A new note was successfully created",
						Type = ToastType.Success
					};

					f.Heimdall().Invocation(
						targetSelector: $"#{ToastManager.Id}",
						swap: HeimdallHtml.Swap.AfterBegin,
						payload: ToastManager.Create(toast),
						wrapInTemplate: false
					);
				});
			}
		}
	}
}
