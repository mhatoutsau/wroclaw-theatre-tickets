namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

public interface IFavoriteRepository
{
    Task<UserFavorite?> GetAsync(Guid userId, Guid showId);
    Task<IEnumerable<Show>> GetUserFavoritesAsync(Guid userId);
    Task<int> GetUserFavoritesCountAsync(Guid userId);
    Task AddAsync(UserFavorite favorite);
    Task RemoveAsync(Guid userId, Guid showId);
    Task<bool> IsFavoriteAsync(Guid userId, Guid showId);
}
