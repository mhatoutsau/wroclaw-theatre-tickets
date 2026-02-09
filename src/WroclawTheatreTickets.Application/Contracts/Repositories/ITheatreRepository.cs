namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

public interface ITheatreRepository
{
    Task<Theatre?> GetByIdAsync(Guid id);
    Task<IEnumerable<Theatre>> GetAllAsync();
    Task<IEnumerable<Theatre>> GetActiveAsync();
    Task<Theatre?> GetByNameAsync(string name);
    Task AddAsync(Theatre theatre);
    Task UpdateAsync(Theatre theatre);
    Task DeleteAsync(Guid id);
}
