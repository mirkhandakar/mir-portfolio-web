using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class ExperienceService : IExperienceService
{
    private readonly ApplicationDbContext _context;

    public ExperienceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Experience>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Experiences
            .AsNoTracking()
            .OrderByDescending(e => e.StartDate)
            .ThenBy(e => e.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Experience?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Experiences
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Experience> CreateAsync(Experience experience, CancellationToken cancellationToken = default)
    {
        _context.Experiences.Add(experience);
        await _context.SaveChangesAsync(cancellationToken);

        return experience;
    }

    public async Task<bool> UpdateAsync(
        int id,
        Experience experience,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.Experiences
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.Title = experience.Title;
        existing.Organization = experience.Organization;
        existing.Location = experience.Location;
        existing.Type = experience.Type;
        existing.StartDate = experience.StartDate;
        existing.EndDate = experience.EndDate;
        existing.Description = experience.Description;
        existing.DisplayOrder = experience.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Experiences
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _context.Experiences.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
