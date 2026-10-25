using Markdig;
using Microsoft.AspNetCore.Html;

namespace HeimdallTemplateApp.Services;

public sealed class MarkdownPageRenderer(IWebHostEnvironment environment)
{
    private readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public async Task<IHtmlContent> RenderAsync(string relativePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        var path = Path.Combine(environment.ContentRootPath, relativePath);
        var markdown = await File.ReadAllTextAsync(path, cancellationToken);
        return new HtmlString(Markdown.ToHtml(markdown, pipeline));
    }
}
