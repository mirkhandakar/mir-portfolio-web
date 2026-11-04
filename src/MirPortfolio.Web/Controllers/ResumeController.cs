using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Controllers;

[Route("resume")]
public class ResumeController : Controller
{
    private readonly IPortfolioService _portfolioService;
    private readonly IResumeStorageService _resumeStorage;

    public ResumeController(
        IPortfolioService portfolioService,
        IResumeStorageService resumeStorage)
    {
        _portfolioService = portfolioService;
        _resumeStorage = resumeStorage;
    }

    [HttpGet("")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Download(CancellationToken cancellationToken)
    {
        var profile = await _portfolioService.GetProfileAsync(cancellationToken);
        var fullPath = _resumeStorage.GetPhysicalPath(profile.ResumePath);

        if (fullPath is null || !System.IO.File.Exists(fullPath))
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return View("Unavailable");
        }

        return PhysicalFile(fullPath, "application/pdf", BuildDownloadName(profile.FullName));
    }

    private static string BuildDownloadName(string? fullName)
    {
        var name = string.IsNullOrWhiteSpace(fullName) ? "resume" : fullName.Trim();
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var safeName = new string(
            name.Select(character => invalidCharacters.Contains(character) ? '-' : character)
                .ToArray())
            .Trim();

        return $"{safeName} Resume.pdf";
    }
}
