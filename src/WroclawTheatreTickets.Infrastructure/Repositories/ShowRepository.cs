namespace WroclawTheatreTickets.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;

public class ShowRepository : IShowRepository
{
    private readonly TheatreDbContext _context;

    public ShowRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<Show?> GetByIdAsync(Guid id)
    {
        return await _context.Shows.FindAsync(id);
    }

    public async Task<Show?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Shows
            .Include(s => s.Theatre)
            .Include(s => s.Reviews)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Show?> GetByExternalIdAsync(string externalId)
    {
        return await _context.Shows
            .Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.ExternalId == externalId);
    }

    public async Task<IEnumerable<Show>> GetAllAsync()
    {
        return await _context.Shows.Include(s => s.Theatre).ToListAsync();
    }

    public async Task<IEnumerable<Show>> GetActiveAsync()
    {
        return await _context.Shows
            .Where(s => s.IsActive)
            .Include(s => s.Theatre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Show>> GetByTheatreIdAsync(Guid theatreId)
    {
        return await _context.Shows
            .Where(s => s.TheatreId == theatreId && s.IsActive)
            .Include(s => s.Theatre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Show>> GetUpcomingAsync(int days = 30)
    {
        var from = DateTime.UtcNow;
        var to = from.AddDays(days);

        return await _context.Shows
            .Where(s => s.IsActive && s.StartDateTime >= from && s.StartDateTime <= to)
            .Include(s => s.Theatre)
            .OrderBy(s => s.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Show>> SearchAsync(string keyword)
    {
        var searchTerm = keyword.ToLower();
        return await _context.Shows
            .Where(s => s.IsActive && (
                s.Title.ToLower().Contains(searchTerm) ||
                s.Director!.ToLower().Contains(searchTerm) ||
                s.Cast!.ToLower().Contains(searchTerm) ||
                s.Theatre!.Name.ToLower().Contains(searchTerm)
            ))
            .Include(s => s.Theatre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Show>> FilterAsync(ShowFilterCriteria criteria)
    {
        var query = _context.Shows.Where(s => s.IsActive).Include(s => s.Theatre).AsQueryable();

        if (!string.IsNullOrEmpty(criteria.Type))
            query = query.Where(s => s.Type.ToString() == criteria.Type);

        if (criteria.DateFrom.HasValue)
            query = query.Where(s => s.StartDateTime >= criteria.DateFrom);

        if (criteria.DateTo.HasValue)
            query = query.Where(s => s.StartDateTime <= criteria.DateTo);

        if (criteria.TheatreId.HasValue)
            query = query.Where(s => s.TheatreId == criteria.TheatreId);

        if (criteria.PriceFrom.HasValue)
            query = query.Where(s => s.MaximumPrice >= criteria.PriceFrom);

        if (criteria.PriceTo.HasValue)
            query = query.Where(s => s.MinimumPrice <= criteria.PriceTo);

        if (!string.IsNullOrEmpty(criteria.Language))
            query = query.Where(s => s.Language == criteria.Language);

        if (!string.IsNullOrEmpty(criteria.AgeRestriction))
            query = query.Where(s => s.AgeRestriction.ToString() == criteria.AgeRestriction);

        return await query.OrderBy(s => s.StartDateTime).ToListAsync();
    }

    public async Task<IEnumerable<Show>> GetMostViewedAsync(int count = 10)
    {
        return await _context.Shows
            .Where(s => s.IsActive)
            .Include(s => s.Theatre)
            .OrderByDescending(s => s.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Show>> GetTopRatedAsync(int count = 10)
    {
        return await _context.Shows
            .Where(s => s.IsActive && s.ReviewCount > 0)
            .Include(s => s.Theatre)
            .OrderByDescending(s => s.Rating)
            .Take(count)
            .ToListAsync();
    }

    public async Task AddAsync(Show show)
    {
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Show show)
    {
        _context.Shows.Update(show);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var show = await GetByIdAsync(id);
        if (show != null)
        {
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutoffDate)
    {
        var showsToDelete = await _context.Shows
            .Where(s => s.StartDateTime < cutoffDate)
            .ToListAsync();

        _context.Shows.RemoveRange(showsToDelete);
        await _context.SaveChangesAsync();

        return showsToDelete.Count;
    }
}
