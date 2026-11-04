using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MirPortfolio.Web.Models;
using MirPortfolio.Web.Services;
using MirPortfolio.Web.ViewModels;

namespace MirPortfolio.Web.Controllers;

[Route("contact")]
public class ContactController : Controller
{
    private const string SuccessMessage =
        "Thanks for reaching out — your message has been sent. I'll get back to you soon.";

    private readonly IContactMessageService _messageService;
    private readonly IPortfolioService _portfolioService;

    public ContactController(
        IContactMessageService messageService,
        IPortfolioService portfolioService)
    {
        _messageService = messageService;
        _portfolioService = portfolioService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Contact";
        ViewData["MetaDescription"] =
            "Get in touch about roles, projects, or collaboration. Send a message and I will reply by email.";

        var profile = await _portfolioService.GetProfileAsync(cancellationToken);

        return View(new ContactViewModel { Profile = profile });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Index(
        ContactViewModel model,
        CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Contact";
        ViewData["MetaDescription"] =
            "Get in touch about roles, projects, or collaboration. Send a message and I will reply by email.";

        if (!string.IsNullOrWhiteSpace(model.Website))
        {
            TempData["ContactSuccess"] = SuccessMessage;
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            model.Profile = await _portfolioService.GetProfileAsync(cancellationToken);
            return View(model);
        }

        var message = new ContactMessage
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Subject = model.Subject.Trim(),
            Body = model.Body.Trim()
        };

        await _messageService.CreateAsync(message, cancellationToken);

        TempData["ContactSuccess"] = SuccessMessage;
        return RedirectToAction(nameof(Index));
    }
}
