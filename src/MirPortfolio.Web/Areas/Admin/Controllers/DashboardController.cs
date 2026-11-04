using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IAdminDashboardService _dashboardService;

    public DashboardController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Dashboard";
        ViewData["Section"] = "Overview";

        var stats = await _dashboardService.GetStatsAsync(cancellationToken);

        return View(stats);
    }
}
