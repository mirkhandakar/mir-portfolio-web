using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.ViewModels.Admin;

public class BlogPostFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    [RegularExpression(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Use lowercase letters, numbers, and single hyphens between words.")]
    [Display(Name = "URL slug")]
    public string? Slug { get; set; }

    [Required, StringLength(500)]
    [Display(Name = "Excerpt")]
    public string Excerpt { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Body")]
    public string Body { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Cover image URL")]
    public string? CoverImageUrl { get; set; }

    [StringLength(300)]
    [Display(Name = "Tags (comma separated)")]
    public string? Tags { get; set; }

    [Display(Name = "Published")]
    public bool IsPublished { get; set; }
}
