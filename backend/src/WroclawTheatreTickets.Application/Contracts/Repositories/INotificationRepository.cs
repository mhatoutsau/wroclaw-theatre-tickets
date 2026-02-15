namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50);
    Task AddAsync(Notification notification);
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid id);
}
