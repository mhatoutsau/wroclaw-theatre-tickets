namespace WroclawTheatreTickets.Infrastructure.Services;

using MediatR;
using Microsoft.Extensions.Logging;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;
using WroclawTheatreTickets.Domain.Entities;

/// <summary>
/// Service for orchestrating theatre repertoire synchronization.
/// Coordinates fetching theatre data, managing theatre entities, and persisting shows to the database.
/// Uses specialized services for data fetching (IRepertoireDataService) and theatre management (ITheatreProviderService).
/// </summary>
public class TheatreRepertoireSyncService : ITheatreRepertoireSyncService
{
    private readonly IRepertoireDataService _dataService;
    private readonly ITheatreProviderService _theatreProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<TheatreRepertoireSyncService> _logger;

    /// <summary>
    /// Initializes a new instance of the TheatreRepertoireSyncService
    /// </summary>
    public TheatreRepertoireSyncService(
        IRepertoireDataService dataService,
        ITheatreProviderService theatreProvider,
        IMediator mediator,
        ILogger<TheatreRepertoireSyncService> logger)
    {
        _dataService = dataService;
        _theatreProvider = theatreProvider;
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Synchronizes theatre repertoire from the Theatre API to the local database
    /// </summary>
    public async Task<TheatreSyncResult> SyncTheatreRepertoireAsync()
    {
        var result = new TheatreSyncResult();

        try
        {
            _logger.LogInformation("Starting theatre repertoire synchronization");

            // Obtain theatre metadata from the data source via interface properties (with fallbacks)
            var theatreName = _dataService.TheatreName ?? "Unknown Theatre";
            var city = _dataService.City ?? string.Empty;
            var address = _dataService.Address ?? string.Empty;

            // Get or create the theatre
            var theatre = await _theatreProvider.GetOrCreateTheatreAsync(theatreName, city, address);
            if (theatre == null)
            {
                result.ErrorMessage = $"Failed to get or create theatre: {theatreName}";
                _logger.LogError(result.ErrorMessage);
                return result;
            }

            // Fetch and map repertoire data from API
            var shows = await _dataService.FetchAndMapRepertoireAsync(theatre.Id, CancellationToken.None);
            if (!shows.Any())
            {
                _logger.LogWarning("No shows to process after fetching and mapping");
                result.TotalEventsProcessed = 0;
                return result;
            }

            _logger.LogInformation("Processing {ShowCount} shows for persistence", shows.Count);
            result.TotalEventsProcessed = shows.Count;

            // Process each show and create/update in database
            foreach (var show in shows)
            {
                try
                {
                    // Convert Show entity to SaveOrUpdateShowCommand and persist
                    var command = MapShowEntityToCommand(show);
                    var commandResult = await _mediator.Send(command);
                    
                    if (commandResult.IsSuccess)
                    {
                        result.SuccessCount++;
                    }
                    else
                    {
                        _logger.LogWarning("Command failed for show {ShowId} ({Title}): {Error}",
                            show.Id, show.Title, commandResult.Error);
                        result.FailureCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process show {ShowId} ({Title})",
                        show.Id, show.Title);
                    result.FailureCount++;
                }
            }

            _logger.LogInformation(
                "Theatre repertoire synchronization completed. Success: {SuccessCount}, Failed: {FailureCount}",
                result.SuccessCount, result.FailureCount);

            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Synchronization failed: {ex.Message}";
            _logger.LogError(ex, "Theatre repertoire synchronization job failed");
            return result;
        }
    }

    /// <summary>
    /// Maps a Show domain entity to SaveOrUpdateShowCommand for persistence
    /// </summary>
    private SaveOrUpdateShowCommand MapShowEntityToCommand(Show show)
    {
        return new SaveOrUpdateShowCommand
        {
            Title = show.Title,
            ExternalId = show.ExternalId,
            StartDateTime = show.StartDateTime,
            DurationMinutes = (int?)show.Duration?.TotalMinutes ?? 0,
            PerformanceType = show.Type,
            AgeRestriction = show.AgeRestriction,
            Language = show.Language ?? "Polish",
            TicketUrl = show.TicketUrl,
            IsBookingDisabled = !show.IsActive,
            TheatreId = show.TheatreId
        };
    }
}
