using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.ViewModels;

public class HomeViewModel
{
    public SiteProfile Profile { get; init; } = new();

    public IReadOnlyList<Project> FeaturedProjects { get; init; } = Array.Empty<Project>();

    public IReadOnlyList<Skill> Skills { get; init; } = Array.Empty<Skill>();

    public IReadOnlyList<Experience> Experiences { get; init; } = Array.Empty<Experience>();
}
