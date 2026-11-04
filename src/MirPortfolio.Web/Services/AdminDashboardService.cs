using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;

namespace MirPortfolio.Web.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var projectCount = await _context.Projects.CountAsync(cancellationToken);
        var featuredProjectCount = await _context.Projects.CountAsync(p => p.IsFeatured, cancellationToken);
        var skillCount = await _context.Skills.CountAsync(cancellationToken);
        var experienceCount = await _context.Experiences.CountAsync(cancellationToken);
        var blogPostCount = await _context.BlogPosts.CountAsync(cancellationToken);
        var unreadMessageCount = await _context.ContactMessages.CountAsync(m => !m.IsRead, cancellationToken);

        return new AdminDashboardStats(
            projectCount,
            featuredProjectCount,
            skillCount,
            experienceCount,
            blogPostCount,
            unreadMessageCount);
    }
}
