using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Data.Seed;

public static class SeedData
{
    private const string AdminRole = "Admin";

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        await SeedIdentityAsync(services, configuration);
        await SeedContentAsync(context);
    }

    private static async Task SeedIdentityAsync(IServiceProvider services, IConfiguration configuration)
    {
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(AdminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("SeedAdmin credentials are not configured; skipping admin user seed.");
            return;
        }

        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
            {
                logger.LogError(
                    "Failed to create the seed admin user: {Errors}",
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(admin, AdminRole))
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }

    private static async Task SeedContentAsync(ApplicationDbContext context)
    {
        if (!await context.SiteProfiles.AnyAsync())
        {
            context.SiteProfiles.Add(new SiteProfile
            {
                FullName = "Your Name",
                Headline = "Software Developer",
                Bio = "TODO: Replace this placeholder with a short professional bio. " +
                      "Describe your focus areas, the kind of problems you enjoy solving, " +
                      "and what you are looking for next.",
                Email = "you@example.com",
                GitHubUrl = "https://github.com/example",
                LinkedInUrl = "https://www.linkedin.com/in/example",
                Location = "City, Country"
            });
        }

        if (!await context.Projects.AnyAsync())
        {
            context.Projects.AddRange(
                new Project
                {
                    Title = "Sample Project One",
                    Slug = "sample-project-one",
                    Summary = "A short, recruiter-friendly summary of what this project does.",
                    Description = "Describe the problem, your approach, the architecture, and the outcome.",
                    TechTags = "ASP.NET Core, C#, SQL Server",
                    RepoUrl = "https://github.com/example/sample-project-one",
                    IsFeatured = true,
                    DisplayOrder = 1
                },
                new Project
                {
                    Title = "Sample Project Two",
                    Slug = "sample-project-two",
                    Summary = "Another placeholder project card ready to be edited in the admin area.",
                    Description = "Replace this with real project details and links.",
                    TechTags = "JavaScript, HTML, CSS",
                    RepoUrl = "https://github.com/example/sample-project-two",
                    DisplayOrder = 2
                });
        }

        if (!await context.Skills.AnyAsync())
        {
            context.Skills.AddRange(
                new Skill { Name = "C#", Category = "Backend", Proficiency = 4, DisplayOrder = 1 },
                new Skill { Name = "ASP.NET Core", Category = "Backend", Proficiency = 4, DisplayOrder = 2 },
                new Skill { Name = "SQL Server", Category = "Data", Proficiency = 3, DisplayOrder = 3 },
                new Skill { Name = "JavaScript", Category = "Frontend", Proficiency = 3, DisplayOrder = 4 },
                new Skill { Name = "Git", Category = "Tooling", Proficiency = 4, DisplayOrder = 5 });
        }

        if (!await context.Experiences.AnyAsync())
        {
            context.Experiences.AddRange(
                new Experience
                {
                    Title = "Software Developer Intern",
                    Organization = "Sample Company",
                    Location = "Remote",
                    Type = ExperienceType.Work,
                    StartDate = new DateOnly(2024, 6, 1),
                    Description = "TODO: Describe your responsibilities and notable achievements.",
                    DisplayOrder = 1
                },
                new Experience
                {
                    Title = "BSc Computer Science",
                    Organization = "Sample University",
                    Type = ExperienceType.Education,
                    StartDate = new DateOnly(2021, 9, 1),
                    EndDate = new DateOnly(2025, 6, 30),
                    Description = "TODO: Add relevant coursework, projects, or honors.",
                    DisplayOrder = 2
                });
        }

        if (!await context.BlogPosts.AnyAsync())
        {
            context.BlogPosts.Add(new BlogPost
            {
                Title = "Welcome to My Portfolio",
                Slug = "welcome-to-my-portfolio",
                Excerpt = "A short placeholder post that can be edited or deleted from the admin area.",
                Body = "## Hello\n\nThis is a sample blog post written in Markdown. " +
                       "Replace it with your first real article.",
                Tags = "meta",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}
