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

public class NotificationRepository : INotificationRepository
{
    private readonly TheatreDbContext _context;

    public NotificationRepository(TheatreDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
        {
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        _context.Notifications.UpdateRange(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
