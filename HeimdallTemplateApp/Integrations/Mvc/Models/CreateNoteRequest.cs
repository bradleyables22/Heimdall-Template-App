using System.ComponentModel.DataAnnotations;

namespace HeimdallTemplateApp.Models;

public sealed class CreateNoteRequest
{
	[StringLength(125, MinimumLength = 5,
		ErrorMessage = "Title must be between 5 and 125 characters long.")]
	public string Title { get; set; } = string.Empty;

	[StringLength(500, MinimumLength = 5,
		ErrorMessage = "Content must be between 5 and 500 characters long.")]
	public string Content { get; set; } = string.Empty;
}
