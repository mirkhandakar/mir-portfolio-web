using System.Xml.Linq;

namespace MirPortfolio.Web.Services;

public class SitemapService : ISitemapService
{
    private static readonly XNamespace SitemapNamespace =
        "http://www.sitemaps.org/schemas/sitemap/0.9";

    private readonly IProjectService _projectService;
    private readonly IBlogPostService _blogPostService;

    public SitemapService(
        IProjectService projectService,
        IBlogPostService blogPostService)
    {
        _projectService = projectService;
        _blogPostService = blogPostService;
    }

    public async Task<string> ToXmlAsync(
        Uri baseUrl,
        CancellationToken cancellationToken = default)
    {
        baseUrl ??= new Uri("https://localhost:5001/");

        var baseRoute = baseUrl.GetLeftPart(UriPartial.Authority);
        var projectRootUrl = $"{baseRoute}/projects";
        var blogRootUrl = $"{baseRoute}/blog";

        var entries = new List<SitemapEntry>
        {
            new(baseUrl.ToString(), null),
            new($"{baseRoute}/Home/About", null),
            new(projectRootUrl, null),
            new(blogRootUrl, null),
            new($"{baseRoute}/Home/Privacy", null),
            new($"{baseRoute}/contact", null)
        };

        var projects = await _projectService.GetAllAsync(cancellationToken);
        var publishedPosts = await _blogPostService.GetPublishedAsync(cancellationToken);

        entries.AddRange(projects.Select(project => new SitemapEntry(
            $"{projectRootUrl}/{project.Slug}",
            project.UpdatedAt)));

        entries.AddRange(publishedPosts.Select(post => new SitemapEntry(
            $"{blogRootUrl}/{post.Slug}",
            post.PublishedAt ?? post.UpdatedAt)));

        var xml = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                SitemapNamespace + "urlset",
                entries.Select(entry => new XElement(
                    SitemapNamespace + "url",
                    new XElement(
                        SitemapNamespace + "loc",
                        entry.Location),
                    entry.LastModified is null
                        ? null
                        : new XElement(
                            SitemapNamespace + "lastmod",
                            entry.LastModified.Value.ToUniversalTime()
                                .ToString("yyyy-MM-ddTHH:mm:ssZ"))))));

        return xml.ToString(SaveOptions.DisableFormatting);
    }
}

public record SitemapEntry(string Location, DateTime? LastModified);
