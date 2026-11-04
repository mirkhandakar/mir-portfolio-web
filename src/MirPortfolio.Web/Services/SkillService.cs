using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class SkillService : ISkillService
{
    private readonly ApplicationDbContext _context;

    public SkillService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .OrderBy(s => s.Category)
            .ThenBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Skills
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Skill> CreateAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return skill;
    }

    public async Task<bool> UpdateAsync(int id, Skill skill, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.Name = skill.Name;
        existing.Category = skill.Category;
        existing.Proficiency = skill.Proficiency;
        existing.DisplayOrder = skill.DisplayOrder;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _context.Skills.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
