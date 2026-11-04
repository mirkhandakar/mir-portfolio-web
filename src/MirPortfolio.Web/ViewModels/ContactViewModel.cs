using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.ViewModels;

public class ContactViewModel : ContactFormViewModel
{
    [ValidateNever]
    public SiteProfile Profile { get; set; } = new();
}
