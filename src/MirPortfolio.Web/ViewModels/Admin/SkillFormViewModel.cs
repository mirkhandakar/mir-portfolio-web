using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.ViewModels.Admin;

public class SkillFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Skill")]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty;

    [Range(1, 5)]
    [Display(Name = "Proficiency (1–5)")]
    public int? Proficiency { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }
}
