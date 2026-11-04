using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.ViewModels;

public class AboutViewModel
{
    public SiteProfile Profile { get; init; } = new();

    public IReadOnlyList<Skill> Skills { get; init; } = Array.Empty<Skill>();

    public IReadOnlyList<Experience> Experiences { get; init; } = Array.Empty<Experience>();

    public IReadOnlyList<SkillGroup> SkillGroups =>
        Skills
            .GroupBy(s => s.Category)
            .Select(group => new SkillGroup(group.Key, group.ToList()))
            .ToList();
}

public record SkillGroup(string Category, IReadOnlyList<Skill> Skills);
