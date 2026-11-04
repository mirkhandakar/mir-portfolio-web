using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.Models;

public class BlogPost
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Excerpt { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [StringLength(300)]
    public string? Tags { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
