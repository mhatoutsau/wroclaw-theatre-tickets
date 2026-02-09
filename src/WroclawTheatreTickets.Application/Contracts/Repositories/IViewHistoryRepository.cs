namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

public interface IViewHistoryRepository
{
    Task<IEnumerable<ViewHistory>> GetUserHistoryAsync(Guid userId, int limit = 50);
    Task AddAsync(ViewHistory history);
}
