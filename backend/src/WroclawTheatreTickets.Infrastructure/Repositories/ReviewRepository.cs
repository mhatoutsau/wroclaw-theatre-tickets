namespace WroclawTheatreTickets.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;

public class ReviewRepository : IReviewRepository
{
    private readonly TheatreDbContext _context;

    public ReviewRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Review>> GetByShowIdAsync(Guid showId)
    {
        return await _context.Reviews.Where(r => r.ShowId == showId).Include(r => r.User).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetApprovedByShowIdAsync(Guid showId)
    {
        return await _context.Reviews
            .Where(r => r.ShowId == showId && r.IsApproved)
            .Include(r => r.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
    {
        return await _context.Reviews.Where(r => !r.IsApproved).Include(r => r.User).ToListAsync();
    }

    public async Task<double> GetAverageRatingAsync(Guid showId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.ShowId == showId && r.IsApproved)
            .ToListAsync();

        return reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
