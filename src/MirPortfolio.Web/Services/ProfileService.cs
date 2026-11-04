using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SiteProfile?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SiteProfiles
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SiteProfile> SaveAsync(
        SiteProfile profile,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.SiteProfiles
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing is null)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            _context.SiteProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);

            return profile;
        }

        existing.FullName = profile.FullName;
        existing.Headline = profile.Headline;
        existing.Bio = profile.Bio;
        existing.AvatarUrl = profile.AvatarUrl;
        existing.Email = profile.Email;
        existing.GitHubUrl = profile.GitHubUrl;
        existing.LinkedInUrl = profile.LinkedInUrl;
        existing.Location = profile.Location;
        existing.ResumePath = profile.ResumePath;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return existing;
    }
}
