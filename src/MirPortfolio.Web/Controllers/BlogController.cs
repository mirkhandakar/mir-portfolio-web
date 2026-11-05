using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels;

namespace MirPortfolio.Web.Controllers;

[Route("blog")]
public class BlogController : Controller
{
    private readonly IBlogPostService _blogPostService;
    private readonly IMarkdownSanitizer _markdownSanitizer;

    public BlogController(
        IBlogPostService blogPostService,
        IMarkdownSanitizer markdownSanitizer)
    {
        _blogPostService = blogPostService;
        _markdownSanitizer = markdownSanitizer;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Blog";
        ViewData["MetaDescription"] =
            "Articles on software development, architecture, and lessons learned while building real projects.";

        var posts = await _blogPostService.GetPublishedAsync(cancellationToken);

        return View(posts);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken)
    {
        var post = await _blogPostService.GetPublishedBySlugAsync(slug, cancellationToken);

        if (post is null)
        {
            return NotFound();
        }

        ViewData["Title"] = post.Title;
        ViewData["MetaDescription"] = post.Excerpt;

        return View(new BlogPostDetailViewModel
        {
            Post = post,
            RenderedBody = _markdownSanitizer.ToSanitizedHtml(post.Body)
        });
    }
}
