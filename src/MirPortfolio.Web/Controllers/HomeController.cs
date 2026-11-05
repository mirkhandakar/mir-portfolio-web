using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels;

namespace MirPortfolio.Web.Controllers;

public class HomeController : Controller
{
    private readonly IPortfolioService _portfolioService;

    public HomeController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new HomeViewModel
        {
            Profile = await _portfolioService.GetProfileAsync(cancellationToken),
            FeaturedProjects = await _portfolioService.GetFeaturedProjectsAsync(3, cancellationToken),
            Skills = await _portfolioService.GetSkillsAsync(cancellationToken),
            Experiences = await _portfolioService.GetExperiencesAsync(cancellationToken)
        };

        ViewData["Title"] = "Home";
        ViewData["MetaDescription"] =
            $"{model.Profile.FullName} — {model.Profile.Headline}. View featured projects, skills, experience, and get in touch.";

        return View(model);
    }

    public async Task<IActionResult> About(CancellationToken cancellationToken)
    {
        var model = new AboutViewModel
        {
            Profile = await _portfolioService.GetProfileAsync(cancellationToken),
            Skills = await _portfolioService.GetSkillsAsync(cancellationToken),
            Experiences = await _portfolioService.GetExperiencesAsync(cancellationToken)
        };

        ViewData["Title"] = "About";
        ViewData["MetaDescription"] =
            $"Learn more about {model.Profile.FullName}: {model.Profile.Headline}.";

        return View(model);
    }

    public async Task<IActionResult> Privacy(CancellationToken cancellationToken)
    {
        var profile = await _portfolioService.GetProfileAsync(cancellationToken);

        ViewData["Title"] = "Privacy";
        ViewData["MetaDescription"] = "Privacy information for this portfolio website.";
        ViewData["ContactEmail"] = profile.Email;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        if (statusCode is 404)
        {
            return View("NotFound");
        }

        if (statusCode is 500)
        {
            return View("ServerError", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
