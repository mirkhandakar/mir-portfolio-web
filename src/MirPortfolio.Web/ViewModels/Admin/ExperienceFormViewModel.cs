using System.ComponentModel.DataAnnotations;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.ViewModels.Admin;

public class ExperienceFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    [Display(Name = "Role or qualification")]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Organization")]
    public string Organization { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Location")]
    public string? Location { get; set; }

    [Display(Name = "Type")]
    public ExperienceType Type { get; set; } = ExperienceType.Work;

    [DataType(DataType.Date)]
    [Display(Name = "Start date")]
    public DateOnly StartDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "End date")]
    public DateOnly? EndDate { get; set; }

    [Required]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Display(Name = "Display order")]
    public int DisplayOrder { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate == default)
        {
            yield return new ValidationResult(
                "Start date is required.",
                new[] { nameof(StartDate) });
        }

        if (EndDate.HasValue && EndDate.Value < StartDate)
        {
            yield return new ValidationResult(
                "End date cannot be before the start date.",
                new[] { nameof(EndDate) });
        }
    }
}
