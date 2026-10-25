namespace HeimdallTemplateApp.Models;

public sealed class NotesPageViewModel
{
	public CreateNoteFormViewModel Form { get; init; } = new();
	public IReadOnlyList<NoteEntity> Notes { get; init; } = [];
}
