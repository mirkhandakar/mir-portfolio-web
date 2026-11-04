using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels.Admin;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ExperiencesController : Controller
{
    private readonly IExperienceService _experienceService;

    public ExperiencesController(IExperienceService experienceService)
    {
        _experienceService = experienceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Experience";
        ViewData["Section"] = "Content";

        var experiences = await _experienceService.GetAllAsync(cancellationToken);

        return View(experiences);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "New experience";
        ViewData["Section"] = "Content";

        return View(new ExperienceFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ExperienceFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New experience";
        ViewData["Section"] = "Content";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var experience = ToEntity(model);
        await _experienceService.CreateAsync(experience, cancellationToken);

        TempData["Success"] = $"Experience \"{experience.Title}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit experience";
        ViewData["Section"] = "Content";

        var experience = await _experienceService.GetByIdAsync(id, cancellationToken);

        if (experience is null)
        {
            return NotFound();
        }

        return View(ToForm(experience));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ExperienceFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit experience";
        ViewData["Section"] = "Content";

        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _experienceService.UpdateAsync(id, ToEntity(model), cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Experience \"{model.Title}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Delete experience";
        ViewData["Section"] = "Content";

        var experience = await _experienceService.GetByIdAsync(id, cancellationToken);

        if (experience is null)
        {
            return NotFound();
        }

        return View(experience);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var experience = await _experienceService.GetByIdAsync(id, cancellationToken);

        if (experience is null)
        {
            return NotFound();
        }

        await _experienceService.DeleteAsync(id, cancellationToken);

        TempData["Success"] = $"Experience \"{experience.Title}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static ExperienceFormViewModel ToForm(Experience experience)
    {
        return new ExperienceFormViewModel
        {
            Id = experience.Id,
            Title = experience.Title,
            Organization = experience.Organization,
            Location = experience.Location,
            Type = experience.Type,
            StartDate = experience.StartDate,
            EndDate = experience.EndDate,
            Description = experience.Description,
            DisplayOrder = experience.DisplayOrder
        };
    }

    private static Experience ToEntity(ExperienceFormViewModel model)
    {
        return new Experience
        {
            Title = model.Title.Trim(),
            Organization = model.Organization.Trim(),
            Location = string.IsNullOrWhiteSpace(model.Location) ? null : model.Location.Trim(),
            Type = model.Type,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Description = model.Description.Trim(),
            DisplayOrder = model.DisplayOrder
        };
    }
}
