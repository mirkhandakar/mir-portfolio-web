using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface ISkillService
{
    Task<IReadOnlyList<Skill>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Skill> CreateAsync(Skill skill, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(int id, Skill skill, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
