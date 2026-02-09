namespace WroclawTheatreTickets.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;

public class TheatreRepository : ITheatreRepository
{
    private readonly TheatreDbContext _context;

    public TheatreRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<Theatre?> GetByIdAsync(Guid id)
    {
        return await _context.Theatres.FindAsync(id);
    }

    public async Task<IEnumerable<Theatre>> GetAllAsync()
    {
        return await _context.Theatres.ToListAsync();
    }

    public async Task<IEnumerable<Theatre>> GetActiveAsync()
    {
        return await _context.Theatres.Where(t => t.IsActive).ToListAsync();
    }

    public async Task<Theatre?> GetByNameAsync(string name)
    {
        return await _context.Theatres.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task AddAsync(Theatre theatre)
    {
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Theatre theatre)
    {
        _context.Theatres.Update(theatre);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var theatre = await GetByIdAsync(id);
        if (theatre != null)
        {
            _context.Theatres.Remove(theatre);
            await _context.SaveChangesAsync();
        }
    }
}

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

public class UserRepository : IUserRepository
{
    private readonly TheatreDbContext _context;

    public UserRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByExternalIdAsync(string externalId, string provider)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ExternalId == externalId && u.Provider == provider);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
