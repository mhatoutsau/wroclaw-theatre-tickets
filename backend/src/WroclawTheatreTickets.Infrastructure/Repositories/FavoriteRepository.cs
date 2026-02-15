namespace WroclawTheatreTickets.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly TheatreDbContext _context;

    public FavoriteRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<UserFavorite?> GetAsync(Guid userId, Guid showId)
    {
        return await _context.UserFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ShowId == showId);
    }

    public async Task<IEnumerable<Show>> GetUserFavoritesAsync(Guid userId)
    {
        return await _context.UserFavorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Show)
            .ThenInclude(s => s!.Theatre)
            .Select(f => f.Show!)
            .ToListAsync();
    }

    public async Task<int> GetUserFavoritesCountAsync(Guid userId)
    {
        return await _context.UserFavorites.CountAsync(f => f.UserId == userId);
    }

    public async Task AddAsync(UserFavorite favorite)
    {
        await _context.UserFavorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid userId, Guid showId)
    {
        var favorite = await GetAsync(userId, showId);
        if (favorite != null)
        {
            _context.UserFavorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsFavoriteAsync(Guid userId, Guid showId)
    {
        return await _context.UserFavorites.AnyAsync(f => f.UserId == userId && f.ShowId == showId);
    }
}
