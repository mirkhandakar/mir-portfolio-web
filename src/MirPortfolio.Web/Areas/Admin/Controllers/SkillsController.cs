using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels.Admin;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SkillsController : Controller
{
    private readonly ISkillService _skillService;

    public SkillsController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Skills";
        ViewData["Section"] = "Content";

        var skills = await _skillService.GetAllAsync(cancellationToken);

        return View(skills);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "New skill";
        ViewData["Section"] = "Content";

        return View(new SkillFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SkillFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "New skill";
        ViewData["Section"] = "Content";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var skill = ToEntity(model);
        await _skillService.CreateAsync(skill, cancellationToken);

        TempData["Success"] = $"Skill \"{skill.Name}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit skill";
        ViewData["Section"] = "Content";

        var skill = await _skillService.GetByIdAsync(id, cancellationToken);

        if (skill is null)
        {
            return NotFound();
        }

        return View(ToForm(skill));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SkillFormViewModel model, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Edit skill";
        ViewData["Section"] = "Content";

        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _skillService.UpdateAsync(id, ToEntity(model), cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = $"Skill \"{model.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Delete skill";
        ViewData["Section"] = "Content";

        var skill = await _skillService.GetByIdAsync(id, cancellationToken);

        if (skill is null)
        {
            return NotFound();
        }

        return View(skill);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var skill = await _skillService.GetByIdAsync(id, cancellationToken);

        if (skill is null)
        {
            return NotFound();
        }

        await _skillService.DeleteAsync(id, cancellationToken);

        TempData["Success"] = $"Skill \"{skill.Name}\" was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static SkillFormViewModel ToForm(Skill skill)
    {
        return new SkillFormViewModel
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            Proficiency = skill.Proficiency,
            DisplayOrder = skill.DisplayOrder
        };
    }

    private static Skill ToEntity(SkillFormViewModel model)
    {
        return new Skill
        {
            Name = model.Name.Trim(),
            Category = model.Category.Trim(),
            Proficiency = model.Proficiency,
            DisplayOrder = model.DisplayOrder
        };
    }
}
