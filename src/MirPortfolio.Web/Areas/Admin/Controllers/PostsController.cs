using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels.Admin;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class PostsController : Controller
{
    private readonly IBlogPostService _blogPostService;

    public PostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Blog posts";
        ViewData["Section"] = "Content";

        var posts = await _blogPostService.GetAllForAdminAsync(cancellationToken);

        return View(posts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "New blog post";
        ViewData["Section"] = "Content";

        return View(new BlogPostFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BlogPostFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New blog post";
        ViewData["Section"] = "Content";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Slug = model.Slug?.Trim();

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = await _blogPostService.GenerateUniqueSlugAsync(
                model.Title,
                cancellationToken: cancellationToken);
        }
        else if (await _blogPostService.SlugExistsAsync(
            model.Slug,
            cancellationToken: cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Slug), "Another post already uses this slug.");
            return View(model);
        }

        var post = ToEntity(model);
        await _blogPostService.CreateAsync(post, cancellationToken);

        TempData["Success"] = $"Blog post \"{post.Title}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit blog post";
        ViewData["Section"] = "Content";

        var post = await _blogPostService.GetByIdAsync(id, cancellationToken);

        if (post is null)
        {
            return NotFound();
        }

        return View(ToForm(post));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        BlogPostFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit blog post";
        ViewData["Section"] = "Content";

        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Slug = model.Slug?.Trim();

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = await _blogPostService.GenerateUniqueSlugAsync(
                model.Title,
                id,
                cancellationToken);
        }
        else if (await _blogPostService.SlugExistsAsync(model.Slug, id, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Slug), "Another post already uses this slug.");
            return View(model);
        }

        var updated = await _blogPostService.UpdateAsync(id, ToEntity(model), cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Blog post \"{model.Title}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Delete blog post";
        ViewData["Section"] = "Content";

        var post = await _blogPostService.GetByIdAsync(id, cancellationToken);

        if (post is null)
        {
            return NotFound();
        }

        return View(post);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var post = await _blogPostService.GetByIdAsync(id, cancellationToken);

        if (post is null)
        {
            return NotFound();
        }

        await _blogPostService.DeleteAsync(id, cancellationToken);

        TempData["Success"] = $"Blog post \"{post.Title}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static BlogPostFormViewModel ToForm(BlogPost post)
    {
        return new BlogPostFormViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Excerpt = post.Excerpt,
            Body = post.Body,
            CoverImageUrl = post.CoverImageUrl,
            Tags = post.Tags,
            IsPublished = post.IsPublished
        };
    }

    private static BlogPost ToEntity(BlogPostFormViewModel model)
    {
        return new BlogPost
        {
            Title = model.Title.Trim(),
            Slug = model.Slug!.Trim(),
            Excerpt = model.Excerpt.Trim(),
            Body = model.Body.Trim(),
            CoverImageUrl = string.IsNullOrWhiteSpace(model.CoverImageUrl)
                ? null
                : model.CoverImageUrl.Trim(),
            Tags = string.IsNullOrWhiteSpace(model.Tags) ? null : model.Tags.Trim(),
            IsPublished = model.IsPublished
        };
    }
}
