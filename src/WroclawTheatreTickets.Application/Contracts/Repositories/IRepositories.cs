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

public class ShowFilterCriteria
{
    public string? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? TheatreId { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public string? Language { get; set; }
    public string? AgeRestriction { get; set; }
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByExternalIdAsync(string externalId, string provider);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}

public interface IFavoriteRepository
{
    Task<UserFavorite?> GetAsync(Guid userId, Guid showId);
    Task<IEnumerable<Show>> GetUserFavoritesAsync(Guid userId);
    Task<int> GetUserFavoritesCountAsync(Guid userId);
    Task AddAsync(UserFavorite favorite);
    Task RemoveAsync(Guid userId, Guid showId);
    Task<bool> IsFavoriteAsync(Guid userId, Guid showId);
}

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<IEnumerable<Review>> GetByShowIdAsync(Guid showId);
    Task<IEnumerable<Review>> GetApprovedByShowIdAsync(Guid showId);
    Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Review>> GetPendingReviewsAsync();
    Task<double> GetAverageRatingAsync(Guid showId);
    Task AddAsync(Review review);
    Task UpdateAsync(Review review);
    Task DeleteAsync(Guid id);
}

public interface IViewHistoryRepository
{
    Task<IEnumerable<ViewHistory>> GetUserHistoryAsync(Guid userId, int limit = 50);
    Task AddAsync(ViewHistory history);
}

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int limit = 50);
    Task AddAsync(Notification notification);
    Task MarkAsReadAsync(Guid id);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid id);
}
