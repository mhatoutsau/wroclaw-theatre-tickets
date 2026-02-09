namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

public interface IShowRepository
{
    Task<Show?> GetByIdAsync(Guid id);
    Task<Show?> GetByIdWithDetailsAsync(Guid id);
    Task<Show?> GetByExternalIdAsync(string externalId);
    Task<IEnumerable<Show>> GetAllAsync();
    Task<IEnumerable<Show>> GetActiveAsync();
    Task<IEnumerable<Show>> GetByTheatreIdAsync(Guid theatreId);
    Task<IEnumerable<Show>> GetUpcomingAsync(int days = 30);
    Task<IEnumerable<Show>> SearchAsync(string keyword);
    Task<IEnumerable<Show>> FilterAsync(ShowFilterCriteria criteria);
    Task<IEnumerable<Show>> GetMostViewedAsync(int count = 10);
    Task<IEnumerable<Show>> GetTopRatedAsync(int count = 10);
    Task AddAsync(Show show);
    Task UpdateAsync(Show show);
    Task DeleteAsync(Guid id);
    Task<int> DeleteOlderThanAsync(DateTime cutoffDate);
}
