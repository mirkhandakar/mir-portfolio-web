using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Projects";
        ViewData["MetaDescription"] =
            "Selected software projects, case studies, and the technologies used to build them.";

        var projects = await _projectService.GetAllAsync(cancellationToken);

        return View(projects);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetBySlugAsync(slug, cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        ViewData["Title"] = project.Title;
        ViewData["MetaDescription"] = project.Summary;

        return View(project);
    }
}
