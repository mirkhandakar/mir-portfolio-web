using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class PortfolioService : IPortfolioService
{
    private readonly ApplicationDbContext _context;

    public PortfolioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SiteProfile> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var profile = await _context.SiteProfiles
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return profile ?? CreateFallbackProfile();
    }

    public async Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(
        int count = 3,
        CancellationToken cancellationToken = default)
    {
        var projects = await _context.Projects
            .AsNoTracking()
            .Where(p => p.IsFeatured)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

        if (projects.Count == 0)
        {
            projects = await _context.Projects
                .AsNoTracking()
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        return projects;
    }

    public async Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Experience>> GetExperiencesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Experiences
            .AsNoTracking()
            .OrderByDescending(e => e.StartDate)
            .ThenBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    private static SiteProfile CreateFallbackProfile()
    {
        return new SiteProfile
        {
            FullName = "Your Name",
            Headline = "Software Developer",
            Bio = "Add your professional profile in the admin area to replace this placeholder.",
            Email = "you@example.com"
        };
    }
}
