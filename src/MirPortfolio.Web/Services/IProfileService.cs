using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IProfileService
{
    Task<SiteProfile?> GetAsync(CancellationToken cancellationToken = default);

    Task<SiteProfile> SaveAsync(SiteProfile profile, CancellationToken cancellationToken = default);
}
