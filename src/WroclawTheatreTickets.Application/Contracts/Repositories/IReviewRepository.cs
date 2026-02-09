namespace WroclawTheatreTickets.Application.Contracts.Repositories;

using WroclawTheatreTickets.Domain.Entities;

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
