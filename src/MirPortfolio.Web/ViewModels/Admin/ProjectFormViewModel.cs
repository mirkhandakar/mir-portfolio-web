using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.ViewModels.Admin;

public class ProjectFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(160)]
    [RegularExpression(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Use lowercase letters, numbers, and single hyphens between words.")]
    [Display(Name = "URL slug")]
    public string? Slug { get; set; }

    [Required, StringLength(300)]
    [Display(Name = "Short summary")]
    public string Summary { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [StringLength(300)]
    [Display(Name = "Tech tags (comma separated)")]
    public string? TechTags { get; set; }

    [StringLength(500)]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Repository URL")]
    public string? RepoUrl { get; set; }

    [Url, StringLength(500)]
    [Display(Name = "Live site URL")]
    public string? LiveUrl { get; set; }

    [Display(Name = "Feature on the home page")]
    public bool IsFeatured { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }
}
