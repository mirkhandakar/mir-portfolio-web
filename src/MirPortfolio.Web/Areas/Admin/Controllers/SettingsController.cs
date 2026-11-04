using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels.Admin;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly IProfileService _profileService;
    private readonly IResumeStorageService _resumeStorage;

    public SettingsController(
        IProfileService profileService,
        IResumeStorageService resumeStorage)
    {
        _profileService = profileService;
        _resumeStorage = resumeStorage;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Settings";
        ViewData["Section"] = "Site";

        var profile = await _profileService.GetAsync(cancellationToken) ?? new SiteProfile();

        return View(ToForm(profile));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Index(
        SiteProfileFormViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Settings";
        ViewData["Section"] = "Site";

        var existing = await _profileService.GetAsync(cancellationToken) ?? new SiteProfile();

        if (!ModelState.IsValid)
        {
            ApplyResumeState(model, existing);
            return View(model);
        }

        string? newResumePath = null;

        if (model.ResumeFile is { Length: > 0 })
        {
            var upload = await _resumeStorage.SaveAsync(model.ResumeFile, cancellationToken);

            if (!upload.Succeeded)
            {
                ModelState.AddModelError(nameof(model.ResumeFile), upload.Error ?? "The resume could not be uploaded.");
                ApplyResumeState(model, existing);
                return View(model);
            }

            newResumePath = upload.RelativePath;
        }

        var oldResumePath = existing.ResumePath;

        ApplyTo(existing, model);

        if (newResumePath is not null)
        {
            existing.ResumePath = newResumePath;
        }
        else if (model.RemoveResume)
        {
            existing.ResumePath = null;
        }

        try
        {
            await _profileService.SaveAsync(existing, cancellationToken);
        }
        catch
        {
            if (newResumePath is not null)
            {
                _resumeStorage.Delete(newResumePath);
            }

            throw;
        }

        if (newResumePath is not null || model.RemoveResume)
        {
            _resumeStorage.Delete(oldResumePath);
        }

        TempData["Success"] = "Settings saved.";
        return RedirectToAction(nameof(Index));
    }

    private static void ApplyTo(SiteProfile profile, SiteProfileFormViewModel model)
    {
        profile.FullName = model.FullName.Trim();
        profile.Headline = model.Headline.Trim();
        profile.Bio = model.Bio.Trim();
        profile.AvatarUrl = Normalize(model.AvatarUrl);
        profile.Email = model.Email.Trim();
        profile.GitHubUrl = Normalize(model.GitHubUrl);
        profile.LinkedInUrl = Normalize(model.LinkedInUrl);
        profile.Location = Normalize(model.Location);
    }

    private static SiteProfileFormViewModel ToForm(SiteProfile profile)
    {
        return new SiteProfileFormViewModel
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Headline = profile.Headline,
            Bio = profile.Bio,
            AvatarUrl = profile.AvatarUrl,
            Email = profile.Email,
            GitHubUrl = profile.GitHubUrl,
            LinkedInUrl = profile.LinkedInUrl,
            Location = profile.Location,
            HasResume = !string.IsNullOrWhiteSpace(profile.ResumePath),
            ResumeFileName = string.IsNullOrWhiteSpace(profile.ResumePath)
                ? null
                : Path.GetFileName(profile.ResumePath)
        };
    }

    private static void ApplyResumeState(SiteProfileFormViewModel model, SiteProfile profile)
    {
        model.HasResume = !string.IsNullOrWhiteSpace(profile.ResumePath);
        model.ResumeFileName = model.HasResume
            ? Path.GetFileName(profile.ResumePath)
            : null;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
