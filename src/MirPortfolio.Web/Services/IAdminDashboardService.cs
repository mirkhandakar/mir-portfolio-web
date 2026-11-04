namespace MirPortfolio.Web.Services;

public interface IAdminDashboardService
{
    Task<AdminDashboardStats> GetStatsAsync(CancellationToken cancellationToken = default);
}
