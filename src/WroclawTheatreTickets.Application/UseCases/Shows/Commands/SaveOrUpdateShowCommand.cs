using MediatR;
using Microsoft.Extensions.Logging;
using FluentValidation;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Domain.Common;
using WroclawTheatreTickets.Domain.Entities;

namespace WroclawTheatreTickets.Application.UseCases.Shows.Commands;

/// <summary>
/// Command to save or update a show from external API data
/// Used by the background job to synchronize theatre repertoire
/// </summary>
public class SaveOrUpdateShowCommand : IRequest<Result<ShowDto>>
{
    public string Title { get; set; } = string.Empty;
    public string? ExternalId { get; set; }
    public DateTime StartDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public Guid TheatreId { get; set; }
    public PerformanceType PerformanceType { get; set; }
    public AgeRestriction AgeRestriction { get; set; }
    public string Language { get; set; } = "Polish";
    public string? TicketUrl { get; set; }
    public bool IsBookingDisabled { get; set; }
}

/// <summary>
/// Handler for SaveOrUpdateShowCommand
/// Creates new shows or updates existing ones based on external ID
/// </summary>
public class SaveOrUpdateShowCommandHandler : IRequestHandler<SaveOrUpdateShowCommand, Result<ShowDto>>
{
    private readonly IShowRepository _showRepository;
    private readonly ITheatreRepository _theatreRepository;
    private readonly AutoMapper.IMapper _mapper;
    private readonly ILogger<SaveOrUpdateShowCommandHandler> _logger;

    public SaveOrUpdateShowCommandHandler(
        IShowRepository showRepository,
        ITheatreRepository theatreRepository,
        AutoMapper.IMapper mapper,
        ILogger<SaveOrUpdateShowCommandHandler> logger)
    {
        _showRepository = showRepository;
        _theatreRepository = theatreRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<ShowDto>> Handle(SaveOrUpdateShowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate theatre exists
            var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);
            if (theatre == null)
            {
                _logger.LogWarning("Theatre not found: {TheatreId}", request.TheatreId);
                return Result<ShowDto>.Failure("Theatre not found");
            }

            Show? existingShow = null;

            // Try to find existing show by external ID
            if (!string.IsNullOrWhiteSpace(request.ExternalId))
            {
                existingShow = await _showRepository.GetByExternalIdAsync(request.ExternalId);
            }

            if (existingShow != null)
            {
                // Update existing show
                existingShow.Title = request.Title;
                existingShow.StartDateTime = request.StartDateTime;
                existingShow.Duration = TimeSpan.FromMinutes(request.DurationMinutes);
                existingShow.Type = request.PerformanceType;
                existingShow.AgeRestriction = request.AgeRestriction;
                existingShow.Language = request.Language;
                existingShow.TicketUrl = request.TicketUrl;
                existingShow.IsActive = true;

                await _showRepository.UpdateAsync(existingShow);
                _logger.LogInformation("Updated show {ShowId}: {Title}", existingShow.Id, request.Title);
            }
            else
            {
                // Create new show
                var newShow = new Show
                {
                    Title = request.Title,
                    ExternalId = request.ExternalId,
                    StartDateTime = request.StartDateTime,
                    Duration = TimeSpan.FromMinutes(request.DurationMinutes),
                    TheatreId = request.TheatreId,
                    Type = request.PerformanceType,
                    AgeRestriction = request.AgeRestriction,
                    Language = request.Language,
                    TicketUrl = request.TicketUrl,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _showRepository.AddAsync(newShow);
                _logger.LogInformation("Created new show {ShowId}: {Title}", newShow.Id, request.Title);
            }

            var result = existingShow != null
                ? _mapper.Map<ShowDto>(existingShow)
                : _mapper.Map<ShowDto>(new Show { Title = request.Title });

            return Result<ShowDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving/updating show: {Title}", request.Title);
            return Result<ShowDto>.Failure($"Error saving show: {ex.Message}");
        }
    }
}

/// <summary>
/// Validator for SaveOrUpdateShowCommand
/// </summary>
public class SaveOrUpdateShowCommandValidator : AbstractValidator<SaveOrUpdateShowCommand>
{
    public SaveOrUpdateShowCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title cannot exceed 500 characters");

        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0");

        RuleFor(x => x.TheatreId)
            .NotEmpty().WithMessage("Theatre ID is required");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required")
            .MaximumLength(50).WithMessage("Language cannot exceed 50 characters");
    }
}
