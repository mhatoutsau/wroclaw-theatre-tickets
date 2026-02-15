namespace WroclawTheatreTickets.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;

public class ViewHistoryRepository : IViewHistoryRepository
{
    private readonly TheatreDbContext _context;

    public ViewHistoryRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ViewHistory>> GetUserHistoryAsync(Guid userId, int limit = 50)
    {
        return await _context.ViewHistories
            .Where(v => v.UserId == userId)
            .Include(v => v.Show)
            .ThenInclude(s => s!.Theatre)
            .OrderByDescending(v => v.ViewedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(ViewHistory history)
    {
        await _context.ViewHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }
}
