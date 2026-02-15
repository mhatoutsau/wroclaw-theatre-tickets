namespace WroclawTheatreTickets.Application.UseCases.Reviews.Commands;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.Contracts.Cache;
using WroclawTheatreTickets.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Options;

public record CreateReviewCommand(Guid UserId, CreateReviewRequest Request) : IRequest<ReviewDto>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IShowRepository _showRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IShowRepository showRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _showRepository = showRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.Rating < 1 || request.Request.Rating > 5)
            throw new Exception("Rating must be between 1 and 5");

        var review = Review.Create(request.UserId, request.Request.ShowId, request.Request.Rating, request.Request.Comment);
        await _reviewRepository.AddAsync(review);

        var avgRating = await _reviewRepository.GetAverageRatingAsync(request.Request.ShowId);
        var show = await _showRepository.GetByIdAsync(request.Request.ShowId);
        if (show != null)
        {
            var allReviews = await _reviewRepository.GetApprovedByShowIdAsync(request.Request.ShowId);
            show.UpdateRating(avgRating, allReviews.Count());
            await _showRepository.UpdateAsync(show);

            // Invalidate show-related caches
            if (_cacheOptions.Enabled)
            {
                await InvalidateShowCaches(show.Id, cancellationToken);
            }
        }

        return _mapper.Map<ReviewDto>(review);
    }

    private async Task InvalidateShowCaches(Guid showId, CancellationToken cancellationToken)
    {
        var keysToInvalidate = new[]
        {
            string.Format(CacheKeys.ShowDetail, showId),
            string.Format(CacheKeys.ReviewsForShow, showId)
        };

        foreach (var key in keysToInvalidate)
        {
            await _cacheService.RemoveAsync(key);
        }
    }
}

public record ApproveReviewCommand(Guid ReviewId) : IRequest<Unit>;

public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Unit>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ICacheService _cacheService;
    private readonly CacheOptions _cacheOptions;

    public ApproveReviewCommandHandler(IReviewRepository reviewRepository, ICacheService cacheService, IOptions<CacheOptions> cacheOptions)
    {
        _reviewRepository = reviewRepository;
        _cacheService = cacheService;
        _cacheOptions = cacheOptions.Value;
    }

    public async Task<Unit> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId);
        if (review == null)
            throw new Exception($"Review with id {request.ReviewId} not found");

        review.IsApproved = true;
        await _reviewRepository.UpdateAsync(review);

        // Invalidate caches related to this review and show
        if (_cacheOptions.Enabled)
        {
            await InvalidateReviewCaches(review.ShowId, cancellationToken);
        }

        return Unit.Value;
    }

    private async Task InvalidateReviewCaches(Guid showId, CancellationToken cancellationToken)
    {
        var keysToInvalidate = new[]
        {
            string.Format(CacheKeys.ShowDetail, showId),
            string.Format(CacheKeys.ReviewsForShow, showId)
        };

        foreach (var key in keysToInvalidate)
        {
            await _cacheService.RemoveAsync(key);
        }
    }
}
