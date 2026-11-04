using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MirPortfolio.Web.Services;

namespace MirPortfolio.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class MessagesController : Controller
{
    private readonly IContactMessageService _messageService;

    public MessagesController(IContactMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filter, CancellationToken cancellationToken)
    {
        var unreadOnly = string.Equals(filter, "unread", StringComparison.OrdinalIgnoreCase);

        ViewData["Title"] = "Messages";
        ViewData["Section"] = "Inbox";
        ViewData["UnreadOnly"] = unreadOnly;

        var messages = await _messageService.GetAllAsync(unreadOnly, cancellationToken);

        return View(messages);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Message";
        ViewData["Section"] = "Inbox";

        var message = await _messageService.GetByIdAsync(id, cancellationToken);

        if (message is null)
        {
            return NotFound();
        }

        if (!message.IsRead)
        {
            await _messageService.MarkReadAsync(id, true, cancellationToken);
            message.IsRead = true;
        }

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkUnread(int id, CancellationToken cancellationToken)
    {
        var updated = await _messageService.MarkReadAsync(id, false, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Message marked as unread.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ViewData["Title"] = "Delete message";
        ViewData["Section"] = "Inbox";

        var message = await _messageService.GetByIdAsync(id, cancellationToken);

        if (message is null)
        {
            return NotFound();
        }

        return View(message);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var message = await _messageService.GetByIdAsync(id, cancellationToken);

        if (message is null)
        {
            return NotFound();
        }

        await _messageService.DeleteAsync(id, cancellationToken);

        TempData["Success"] = $"Message from {message.Name} was deleted.";
        return RedirectToAction(nameof(Index));
    }
}
