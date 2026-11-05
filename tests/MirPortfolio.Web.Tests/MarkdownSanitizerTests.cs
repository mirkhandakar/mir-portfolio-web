using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Tests;

public class MarkdownSanitizerTests
{
    [Fact]
    public void ToSanitizedHtml_ConvertsHeadings()
    {
        var html = new MarkdownSanitizer().ToSanitizedHtml("## Heading");

        Assert.Contains("<h2", html);

        Assert.Contains("Heading", html);
    }

    [Fact]
    public void ToSanitizedHtml_ConvertsLinks()
    {
        var html = new MarkdownSanitizer().ToSanitizedHtml("[Example](https://example.com)");

        Assert.Contains("<a", html);
        Assert.Contains("Example", html);
    }

    [Fact]
    public void ToSanitizedHtml_RemovesInlineScriptContent()
    {
        var html = new MarkdownSanitizer()
            .ToSanitizedHtml("<script>alert(42)</script>");

        Assert.DoesNotContain("<script", html);

        Assert.DoesNotContain("alert(42)", html);
    }

    [Fact]
    public void ToSanitizedHtml_RemovesInlineEventHandlers()
    {
        var html = new MarkdownSanitizer()
            .ToSanitizedHtml(
                "<img src=\"https://example.com/hero.png\" onerror=\"alert(42)\">");

        Assert.DoesNotContain("onerror=", html);
        Assert.DoesNotContain("alert(42)", html);
    }
}
