using System.ComponentModel.DataAnnotations;

namespace MirPortfolio.Web.ViewModels;

public class ContactFormViewModel
{
    [Required, StringLength(100)]
    [Display(Name = "Your name")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(200)]
    [Display(Name = "Subject")]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(4000)]
    [Display(Name = "Message")]
    public string Body { get; set; } = string.Empty;

    public string? Website { get; set; }
}
