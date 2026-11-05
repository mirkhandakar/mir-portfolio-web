using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class BlogPostService : IBlogPostService
{
    private static readonly Regex InvalidSlugCharacters = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex Whitespace = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex RepeatingHyphens = new("-{2,}", RegexOptions.Compiled);

    private readonly ApplicationDbContext _context;

    public BlogPostService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BlogPost>> GetAllForAdminAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts
            .AsNoTracking()
            .OrderByDescending(p => p.IsPublished)
            .ThenByDescending(p => p.UpdatedAt)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BlogPost>> GetPublishedAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BlogPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<BlogPost?> GetPublishedBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        return await _context.BlogPosts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished, cancellationToken);
    }

    public async Task<BlogPost> CreateAsync(
        BlogPost post,
        CancellationToken cancellationToken = default)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdatedAt = post.CreatedAt;
        NormalizePublishing(post);

        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        return post;
    }

    public async Task<bool> UpdateAsync(
        int id,
        BlogPost post,
        CancellationToken cancellationToken = default)
    {
        var existing = await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        existing.Title = post.Title;
        existing.Slug = post.Slug;
        existing.Excerpt = post.Excerpt;
        existing.Body = post.Body;
        existing.CoverImageUrl = post.CoverImageUrl;
        existing.Tags = post.Tags;
        existing.IsPublished = post.IsPublished;
        existing.UpdatedAt = DateTime.UtcNow;

        if (post.IsPublished)
        {
            existing.PublishedAt ??= DateTime.UtcNow;
        }
        else
        {
            existing.PublishedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _context.BlogPosts
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing is null)
        {
            return false;
        }

        _context.BlogPosts.Remove(existing);
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

        return await _context.BlogPosts
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

    private static void NormalizePublishing(BlogPost post)
    {
        if (!post.IsPublished)
        {
            post.PublishedAt = null;
        }
        else if (post.PublishedAt is null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
    }

    private static string Slugify(string title)
    {
        var normalized = (title ?? string.Empty).Trim().ToLowerInvariant();
        normalized = InvalidSlugCharacters.Replace(normalized, string.Empty);
        normalized = Whitespace.Replace(normalized, "-");
        normalized = RepeatingHyphens.Replace(normalized, "-").Trim('-');

        if (normalized.Length > 180)
        {
            normalized = normalized[..180].Trim('-');
        }

        return string.IsNullOrWhiteSpace(normalized) ? "post" : normalized;
    }
}
