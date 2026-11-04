using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IProjectService
{
    Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Project?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, Project project, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<string> GenerateUniqueSlugAsync(
        string title,
        int? excludeId = null,
        CancellationToken cancellationToken = default);
}
