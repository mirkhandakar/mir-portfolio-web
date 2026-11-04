using MirPortfolio.Web.Models;

namespace MirPortfolio.Web.Services;

public interface IContactMessageService
{
    Task<ContactMessage> CreateAsync(
        ContactMessage message,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContactMessage>> GetAllAsync(
        bool unreadOnly = false,
        CancellationToken cancellationToken = default);

    Task<ContactMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> MarkReadAsync(
        int id,
        bool isRead,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);
}
