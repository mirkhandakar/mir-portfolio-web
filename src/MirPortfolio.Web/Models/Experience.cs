using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.Models;

public enum ExperienceType
{
    Work = 1,
    Education = 2
}

public class Experience
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Organization { get; set; } = string.Empty;

    [StringLength(150)]
    public string? Location { get; set; }

    public ExperienceType Type { get; set; } = ExperienceType.Work;

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }
}
