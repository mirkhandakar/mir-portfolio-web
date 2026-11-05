using System.Text;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Controllers;

public class SeoController : Controller
{
    private readonly ISitemapService _sitemapService;

    public SeoController(ISitemapService sitemapService)
    {
        _sitemapService = sitemapService;
    }

    [Route("robots.txt")]
    [HttpGet]
    [ResponseCache(Duration = 604800, Location = ResponseCacheLocation.Any)]
    public IActionResult Robots()
    {
        var sitemapUrl = $"{Request.Scheme}://{Request.Host}/sitemap.xml";
        var content = $"""
            User-agent: *
            Allow: /

            Disallow: /admin
            Disallow: /Identity

            Sitemap: {sitemapUrl}
            """;

        return Content(content, "text/plain", Encoding.UTF8);
    }

    [Route("sitemap.xml")]
    [HttpGet]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Sitemap(CancellationToken cancellationToken)
    {
        var baseUrl = new Uri(
            $"{Request.Scheme}://{Request.Host}{Request.PathBase}/",
            UriKind.Absolute);

        var content = await _sitemapService.ToXmlAsync(baseUrl, cancellationToken);

        return Content(content, "application/xml", Encoding.UTF8);
    }
}
