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
