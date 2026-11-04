using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class ProjectService : IProjectService
{
    private static readonly Regex InvalidSlugCharacters = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex RepeatingHyphens = new("-{2,}", RegexOptions.Compiled);

    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        project.CreatedAt = DateTime.UtcNow;
        project.UpdatedAt = project.CreatedAt;

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        return project;
    }

    public async Task<bool> UpdateAsync(int id, Project project, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.Title = project.Title;
        existing.Slug = project.Slug;
        existing.Summary = project.Summary;
        existing.Description = project.Description;
        existing.TechTags = project.TechTags;
        existing.ImageUrl = project.ImageUrl;
        existing.RepoUrl = project.RepoUrl;
        existing.LiveUrl = project.LiveUrl;
        existing.IsFeatured = project.IsFeatured;
        existing.DisplayOrder = project.DisplayOrder;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _context.Projects.Remove(existing);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return false;
        }

        return await _context.Projects
            .AnyAsync(
                p => p.Slug == slug && (excludeId == null || p.Id != excludeId),
                cancellationToken);
    }

    public async Task<string> GenerateUniqueSlugAsync(
        string title,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var baseSlug = Slugify(title);
        var slug = baseSlug;
        var suffix = 2;

        while (await SlugExistsAsync(slug, excludeId, cancellationToken))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static string Slugify(string title)
    {
        var normalized = (title ?? string.Empty).Trim().ToLowerInvariant();
        normalized = InvalidSlugCharacters.Replace(normalized, string.Empty);
        normalized = Whitespace.Replace(normalized, "-");
        normalized = RepeatingHyphens.Replace(normalized, "-").Trim('-');

        if (normalized.Length > 140)
        {
            normalized = normalized[..140].Trim('-');
        }

        return string.IsNullOrWhiteSpace(normalized) ? "project" : normalized;
    }
}
