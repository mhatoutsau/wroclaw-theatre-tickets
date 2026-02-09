namespace WroclawTheatreTickets.Application.UseCases.Reviews.Commands;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using AutoMapper;

public record CreateReviewCommand(Guid UserId, CreateReviewRequest Request) : IRequest<ReviewDto>;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IShowRepository showRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _showRepository = showRepository;
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
        }

        return _mapper.Map<ReviewDto>(review);
    }
}

public record ApproveReviewCommand(Guid ReviewId) : IRequest<Unit>;

public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Unit>
{
    private readonly IReviewRepository _reviewRepository;

    public ApproveReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Unit> Handle(ApproveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId);
        if (review == null)
            throw new Exception($"Review with id {request.ReviewId} not found");

        review.IsApproved = true;
        await _reviewRepository.UpdateAsync(review);

        return Unit.Value;
    }
}
