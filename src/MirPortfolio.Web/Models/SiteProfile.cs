using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.Models;

public class SiteProfile
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public string Bio { get; set; } = string.Empty;

    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Url, StringLength(300)]
    public string? GitHubUrl { get; set; }

    [Url, StringLength(300)]
    public string? LinkedInUrl { get; set; }

    [StringLength(150)]
    public string? Location { get; set; }

    [StringLength(300)]
    public string? ResumePath { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
