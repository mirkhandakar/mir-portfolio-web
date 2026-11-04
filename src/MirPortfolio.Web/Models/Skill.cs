using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.Models;

public class Skill
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Category { get; set; } = string.Empty;

    [Range(1, 5)]
    public int? Proficiency { get; set; }

    public int DisplayOrder { get; set; }
}
