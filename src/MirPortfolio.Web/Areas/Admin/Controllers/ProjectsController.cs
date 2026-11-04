using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels.Admin;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Projects";
        ViewData["Section"] = "Content";

        var projects = await _projectService.GetAllAsync(cancellationToken);

        return View(projects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "New project";
        ViewData["Section"] = "Content";

        return View(new ProjectFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New project";
        ViewData["Section"] = "Content";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Slug = model.Slug?.Trim();

        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = await _projectService.GenerateUniqueSlugAsync(
                model.Title,
                cancellationToken: cancellationToken);
        }
        else if (await _projectService.SlugExistsAsync(model.Slug, cancellationToken: cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Slug), "Another project already uses this slug.");
            return View(model);
        }

        var project = ToEntity(model);
        await _projectService.CreateAsync(project, cancellationToken);

        TempData["Success"] = $"Project \"{project.Title}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit project";
        ViewData["Section"] = "Content";

        var project = await _projectService.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return View(ToForm(project));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit project";
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
            model.Slug = await _projectService.GenerateUniqueSlugAsync(
                model.Title,
                id,
                cancellationToken);
        }
        else if (await _projectService.SlugExistsAsync(model.Slug, id, cancellationToken))
        {
            ModelState.AddModelError(nameof(model.Slug), "Another project already uses this slug.");
            return View(model);
        }

        var updated = await _projectService.UpdateAsync(id, ToEntity(model), cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Project \"{model.Title}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Delete project";
        ViewData["Section"] = "Content";

        var project = await _projectService.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return View(project);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        await _projectService.DeleteAsync(id, cancellationToken);

        TempData["Success"] = $"Project \"{project.Title}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static ProjectFormViewModel ToForm(Project project)
    {
        return new ProjectFormViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Slug = project.Slug,
            Summary = project.Summary,
            Description = project.Description,
            TechTags = project.TechTags,
            ImageUrl = project.ImageUrl,
            RepoUrl = project.RepoUrl,
            LiveUrl = project.LiveUrl,
            IsFeatured = project.IsFeatured,
            DisplayOrder = project.DisplayOrder
        };
    }

    private static Project ToEntity(ProjectFormViewModel model)
    {
        return new Project
        {
            Title = model.Title.Trim(),
            Slug = model.Slug!.Trim(),
            Summary = model.Summary.Trim(),
            Description = model.Description.Trim(),
            TechTags = string.IsNullOrWhiteSpace(model.TechTags) ? null : model.TechTags.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
            RepoUrl = string.IsNullOrWhiteSpace(model.RepoUrl) ? null : model.RepoUrl.Trim(),
            LiveUrl = string.IsNullOrWhiteSpace(model.LiveUrl) ? null : model.LiveUrl.Trim(),
            IsFeatured = model.IsFeatured,
            DisplayOrder = model.DisplayOrder
        };
    }
}
