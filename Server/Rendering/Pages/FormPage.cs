using Heimdall.Server;
using Microsoft.AspNetCore.Html;
using Server.Heimdall;          // FluentHeimdall extensions (.Heimdall())
using Server.Rendering.Shared;
using Server.Utilities;
using System.ComponentModel.DataAnnotations;
using static Server.Utilities.Html;

namespace Server.Rendering.Pages
{
    public static class FormPage
    {
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
            // Used only for UX: don’t show errors until user has interacted.
            public bool IsDirty { get; set; }

            [StringLength(125, MinimumLength = 5,
                ErrorMessage = "Title must be between 5 and 125 characters long.")]
            public string Title { get; set; } = string.Empty;

            [StringLength(500, MinimumLength = 5,
                ErrorMessage = "Content must be between 5 and 500 characters long.")]
            public string Content { get; set; } = string.Empty;
        }

        public static IHtmlContent Render()
        {
            // Initial render: don’t show errors until the user types.
            var request = new CreateNoteRequest { IsDirty = false };
            var results = ValidateInternal(Normalize(request));

            return FluentHtml.Div(row =>
            {
                row.Class(Bootstrap.Layout.Row, Bootstrap.Spacing.Mt(4));

                row.Div(col1 =>
                {
                    col1.Class(Bootstrap.Layout.ColSpan(3, Bootstrap.Breakpoint.Lg));
                });

                row.Div(col2 =>
                {
                    col2.Class(Bootstrap.Layout.ColSpan(6, Bootstrap.Breakpoint.Lg));

                    col2.Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

                        card.Div(header =>
                        {
                            header.Class(Bootstrap.Card.Header);
                            header.H2(h => h.Text("Create New Note"));
                        });

                        card.Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body);

                            body.Div(host =>
                            {
                                host.Id("create-note-host");
                                host.Add(GenerateForm(request, results));
                            });
                        });
                    });

                    col2.Div(sp => sp.Class(Bootstrap.Spacing.Mt(4)));

                    col2.Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Lg);

                        card.Div(header =>
                        {
                            header.Class(Bootstrap.Card.Header);
                            header.H2(h => h.Text("Notes"));
                        });

                        card.Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body);

                            body.Div(host =>
                            {
                                host.Id("notes-host");
                                host.Add(RenderNotesList());
                            });
                        });
                    });
                });

                row.Div(col3 =>
                {
                    col3.Class(Bootstrap.Layout.ColSpan(3, Bootstrap.Breakpoint.Lg));
                });
            });
        }

        private static IHtmlContent GenerateForm(CreateNoteRequest req, List<ValidationResult> results)
        {
            var titleError = FirstErrorFor(results, nameof(CreateNoteRequest.Title));
            var contentError = FirstErrorFor(results, nameof(CreateNoteRequest.Content));

            // Only show field errors after first interaction
            var showErrors = req.IsDirty;

            // Submit disabled until truly valid
            var isValid = !results.Any();

            return FluentHtml.Form(form =>
            {
                form.Id("create-note-form");

                // Submit -> CreateNote
                // Target the host but swap INNER to avoid replacing the host node.
                form.Heimdall()
                    .Submit(new HeimdallHtml.ActionId("FormPage.CreateNote"))
                    .PayloadFromClosestForm()
                    .Target("#create-note-host")
                    .SwapInner()
                    .PreventDefault(true);

                // Hidden field so Validated round-trips through closest-form payload
                form.Input(InputType.hidden, h =>
                {
                    h.Name(nameof(CreateNoteRequest.IsDirty));
                    h.Value(req.IsDirty ? "true" : "false");
                });
                form.Label(l =>
                {
                    l.Class(Bootstrap.Form.Label);
                    l.For("title");
                    l.Text("Title");
                });

                form.Input(InputType.text, input =>
                {
                    input.Id("title");
                    input.Name(nameof(CreateNoteRequest.Title));
                    input.Value(req.Title ?? string.Empty);
                    input.Attr("maxlength", "125");

                    // Validate on input with debounce
                    // Swap INNER so we update the form contents without replacing the host node.
                    input.Heimdall()
                        .Input(new HeimdallHtml.ActionId("FormPage.Validate"))
                        .PayloadFromClosestForm()
                        .Target("#create-note-host")
                        .SwapInner()
                        .DebounceMs(250);

                    input.Class(Bootstrap.Form.Control, Bootstrap.Spacing.Mb(1));
                });

                if (showErrors && titleError != null)
                {
                    form.Div(err =>
                    {
                        err.Class(Bootstrap.Form.Text, Bootstrap.Text.TxtColor(Bootstrap.Color.Danger), Bootstrap.Spacing.Mb(3));
                        err.Text(titleError);
                    });
                }
                else
                {
                    form.Div(sp => sp.Class(Bootstrap.Spacing.Mb(3)));
                }
                form.Label(l =>
                {
                    l.Class(Bootstrap.Form.Label);
                    l.For("content");
                    l.Text("Content");
                });

                // Assumes you added FluentHtml.TextArea(...) wrapper previously.
                // If not, you can replace with form.Tag("textarea", ...)
                form.TextArea(input =>
                {
                    input.Id("content");
                    input.Name(nameof(CreateNoteRequest.Content));
                    input.Text(req.Content ?? string.Empty); 
                    input.Attr("maxlength", "500");
                    input.Attr("rows", "5");

                    input.Heimdall()
                        .Input(new HeimdallHtml.ActionId("FormPage.Validate"))
                        .PayloadFromClosestForm()
                        .Target("#create-note-host")
                        .SwapInner()
                        .DebounceMs(250);

                    input.Class(Bootstrap.Form.Control, Bootstrap.Spacing.Mb(1));
                });

                if (showErrors && contentError != null)
                {
                    form.Div(err =>
                    {
                        err.Class(Bootstrap.Form.Text, Bootstrap.Text.TxtColor(Bootstrap.Color.Danger), Bootstrap.Spacing.Mb(3));
                        err.Text(contentError);
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
                        full.Class(Bootstrap.Layout.ColSpan(12, Bootstrap.Breakpoint.Lg));
                        full.Add(SubmitButton(isValid));
                    });
                });
            });
        }

        private static IHtmlContent SubmitButton(bool isValid)
        {
            return FluentHtml.Button(btn =>
            {
                btn.Class(Bootstrap.Btn.Primary, Bootstrap.Sizing.W100);
                btn.Attr("type", "submit");

                if (!isValid)
                {
                    btn.Class(Bootstrap.Btn.Disabled);
                    btn.Attr("disabled", "disabled");
                }

                btn.Text("Submit");
            });
        }

        private static IHtmlContent RenderNotesList()
        {
            if (Notes.Count == 0)
            {
                return FluentHtml.Div(d =>
                {
                    d.Class(Bootstrap.Text.BodySecondary);
                    d.Text("No notes yet.");
                });
            }

            var ordered = Notes
                .OrderByDescending(x => x.CreatedUtc)
                .ToList();

            return FluentHtml.Div(list =>
            {
                list.Class(Bootstrap.Helpers.VStack, Bootstrap.Spacing.Gap(2));

                foreach (var note in ordered)
                {
                    list.Div(card =>
                    {
                        card.Class(Bootstrap.Card.Base, Bootstrap.Shadow.Sm);

                        card.Div(body =>
                        {
                            body.Class(Bootstrap.Card.Body);

                            body.H5(h =>
                            {
                                h.Class(Bootstrap.Card.Title);
                                h.Text(note.Title);
                            });

                            body.Div(s =>
                            {
                                s.Class(Bootstrap.Text.BodySecondary, Bootstrap.Text.Small, Bootstrap.Spacing.Mb(2));
                                s.Text($"Created {note.CreatedUtc:yyyy-MM-dd HH:mm} UTC");
                            });

                            body.Div(p =>
                            {
                                p.Class(Bootstrap.Card.Text);
                                p.Text(note.Content);
                            });
                        });
                    });
                }
            });
        }


        // Called on every keystroke (debounced). Re-renders only the form host.
        [ContentInvocation]
        public static IHtmlContent Validate(CreateNoteRequest draft)
        {
            draft ??= new CreateNoteRequest();

            // Once they type, we start showing errors
            draft.IsDirty = true;

            draft = Normalize(draft);
            var results = ValidateInternal(draft);

            return GenerateForm(draft, results);
        }

        // Called on submit. Re-validates, creates, clears, and OOB-updates notes list.
        [ContentInvocation]
        public static IHtmlContent CreateNote(CreateNoteRequest noteRequest)
        {
            noteRequest ??= new CreateNoteRequest();
            noteRequest.IsDirty = true;

            noteRequest = Normalize(noteRequest);
            var results = ValidateInternal(noteRequest);

            if (!results.Any())
            {
                Notes.Add(new NoteEntity
                {
                    Title = noteRequest.Title,
                    Content = noteRequest.Content
                });

                // Clear form after success, and reset "Validated" so we don't show errors on empty.
                noteRequest = new CreateNoteRequest { IsDirty= false };
                results = ValidateInternal(Normalize(noteRequest));
            }

            // Main swap updates #create-note-host (inner swap),
            // and we also push an OOB invocation for #notes-host and toast-manager
            return FluentHtml.Fragment(f =>
            {
                // Main content for the target: just the form markup
                f.Add(GenerateForm(noteRequest, results));

                f.Heimdall().Invocation(
                    targetSelector: "#notes-host",
                    swap: HeimdallHtml.Swap.Inner,
                    payload: RenderNotesList(),
                    wrapInTemplate: false
                );

                var toast = new ToastItem
                {
                    Header = "Note Created",
                    Content = "A new note was successfully created",
                    Type = ToastType.Success
                };

                f.Heimdall().Invocation(
                    targetSelector: "#toast-manager",
                    swap: HeimdallHtml.Swap.AfterBegin,
                    payload: ToastManager.Create(toast),
                    wrapInTemplate: false
                );

            });
        }

        // ------------------------------------------------------------
        // Normalization + Validation
        // ------------------------------------------------------------
        private static CreateNoteRequest Normalize(CreateNoteRequest req)
        {
            req.Title = (req.Title ?? string.Empty).Trim();
            req.Content = (req.Content ?? string.Empty).Trim();
            return req;
        }

        private static List<ValidationResult> ValidateInternal(CreateNoteRequest req)
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

                // avoid LINQ allocations; MemberNames is IEnumerable<string>
                foreach (var m in r.MemberNames)
                {
                    if (string.Equals(m, memberName, StringComparison.OrdinalIgnoreCase))
                        return r.ErrorMessage;
                }
            }

            return null;
        }
    }
}
