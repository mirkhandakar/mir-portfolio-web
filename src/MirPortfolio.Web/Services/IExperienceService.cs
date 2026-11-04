using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IExperienceService
{
    Task<IReadOnlyList<Experience>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Experience?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Experience> CreateAsync(Experience experience, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, Experience experience, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
