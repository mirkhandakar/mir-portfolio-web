using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SiteProfile> SiteProfiles => Set<SiteProfile>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Project>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<BlogPost>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Experience>()
            .Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
