using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.Models;

public class Project
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(160)]
    public string Slug { get; set; } = string.Empty;

    [Required, StringLength(300)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [StringLength(300)]
    public string? TechTags { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Url, StringLength(500)]
    public string? RepoUrl { get; set; }

    [Url, StringLength(500)]
    public string? LiveUrl { get; set; }

    public bool IsFeatured { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
