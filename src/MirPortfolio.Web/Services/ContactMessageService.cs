using Microsoft.EntityFrameworkCore;
using MirPortfolio.Web.Data;
using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public class ContactMessageService : IContactMessageService
{
    private readonly ApplicationDbContext _context;

    public ContactMessageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ContactMessage> CreateAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default)
    {
        if (message.CreatedAt == default)
        {
            message.CreatedAt = DateTime.UtcNow;
        }

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return message;
    }

    public async Task<IReadOnlyList<ContactMessage>> GetAllAsync(
        bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ContactMessages.AsNoTracking();

        if (unreadOnly)
        {
            query = query.Where(m => !m.IsRead);
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ContactMessage?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ContactMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> MarkReadAsync(
        int id,
        bool isRead,
        CancellationToken cancellationToken = default)
    {
        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (message is null)
        {
            return false;
        }

        message.IsRead = isRead;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await _context.ContactMessages
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (message is null)
        {
            return false;
        }

        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ContactMessages
            .CountAsync(m => !m.IsRead, cancellationToken);
    }
}
