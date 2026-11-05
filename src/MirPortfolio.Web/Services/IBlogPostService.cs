using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IBlogPostService
{
    Task<IReadOnlyList<BlogPost>> GetAllForAdminAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlogPost>> GetPublishedAsync(CancellationToken cancellationToken = default);

    Task<BlogPost?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetPublishedBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default);

    Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        BlogPost post,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    Task<string> GenerateUniqueSlugAsync(
        string title,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}
