using Ganss.Xss;
using Markdig;

namespace MirPortfolio.Web.Services;

public class MarkdownSanitizer : IMarkdownSanitizer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public string ToSanitizedHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return string.Empty;
        }

        var html = Markdown.ToHtml(markdown, Pipeline);
        var sanitizer = new HtmlSanitizer();

        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.UnionWith(new[]
        {
            "a", "b", "blockquote", "br", "code", "del", "details", "em", "figure",
            "figcaption", "h1", "h2", "h3", "h4", "h5", "h6", "hr", "i", "img",
            "kbd", "li", "mark", "ol", "p", "pre", "q", "s", "small", "span",
            "strike", "strong", "sub", "summary", "sup", "table", "tbody", "td",
            "th", "thead", "tr", "u", "ul"
        });

        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.UnionWith(new[]
        {
            "alt", "colspan", "height", "href", "id", "lang", "rel", "rowspan",
            "src", "title", "width"
        });

        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.UnionWith(new[]
        {
            "http", "https", "mailto"
        });

        return sanitizer.Sanitize(html);
    }
}
