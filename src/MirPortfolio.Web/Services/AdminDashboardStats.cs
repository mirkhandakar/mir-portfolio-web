namespace MirPortfolio.Web.Services;

public record AdminDashboardStats(
    int ProjectCount,
    int FeaturedProjectCount,
    int SkillCount,
    int ExperienceCount,
    int BlogPostCount,
    int UnreadMessageCount);
