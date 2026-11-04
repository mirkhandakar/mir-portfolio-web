using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IPortfolioService
{
    Task<SiteProfile> GetProfileAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(
        int count = 3,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Experience>> GetExperiencesAsync(CancellationToken cancellationToken = default);
}
