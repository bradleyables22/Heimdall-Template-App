namespace HeimdallTemplateApp.Models;

public sealed class CreateNoteFormViewModel
{
	public CreateNoteRequest Request { get; init; } = new();
	public IReadOnlyDictionary<string, string> Errors { get; init; } =
		new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	public bool ShowErrors { get; init; }

	public string? ErrorFor(string fieldName) =>
		Errors.TryGetValue(fieldName, out var error) ? error : null;
}
