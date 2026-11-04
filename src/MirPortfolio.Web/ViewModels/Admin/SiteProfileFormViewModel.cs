using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.ViewModels.Admin;

public class SiteProfileFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Headline")]
    public string Headline { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Bio")]
    public string Bio { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Avatar URL")]
    public string? AvatarUrl { get; set; }

    [Required, EmailAddress, StringLength(200)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Url, StringLength(300)]
    [Display(Name = "GitHub URL")]
    public string? GitHubUrl { get; set; }

    [Url, StringLength(300)]
    [Display(Name = "LinkedIn URL")]
    public string? LinkedInUrl { get; set; }

    [StringLength(150)]
    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "Resume PDF")]
    public IFormFile? ResumeFile { get; set; }

    [Display(Name = "Remove the current resume")]
    public bool RemoveResume { get; set; }

    public bool HasResume { get; set; }

    public string? ResumeFileName { get; set; }
}
