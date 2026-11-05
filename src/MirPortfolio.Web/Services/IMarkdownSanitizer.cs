using Ganss.Xss;
using Markdig;

namespace MirPortfolio.Web.Services;

public interface IMarkdownSanitizer
{
    string ToSanitizedHtml(string markdown);
}
