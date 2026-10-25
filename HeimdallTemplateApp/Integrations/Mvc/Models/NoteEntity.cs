namespace HeimdallTemplateApp.Models;

public sealed class NoteEntity
{
	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string Title { get; init; } = string.Empty;
	public string Content { get; init; } = string.Empty;
	public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
}
