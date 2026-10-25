namespace HeimdallTemplateApp.Models;

public sealed class CreateNoteFormViewModel
{
    public CreateNoteRequest Request { get; init; } = new();
    public bool ShowErrors { get; init; }
    public IReadOnlyDictionary<string, string> Errors { get; init; }
        = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public string? ErrorFor(string name)
        => Errors.TryGetValue(name, out var error) ? error : null;
}
