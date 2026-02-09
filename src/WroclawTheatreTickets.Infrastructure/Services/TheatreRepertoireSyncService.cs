namespace WroclawTheatreTickets.Infrastructure.Services;

using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;

/// <summary>
/// Service for synchronizing theatre repertoire data from the Theatre API
/// to the local database. Handles API communication and data persistence.
/// </summary>
public class TheatreRepertoireSyncService : ITheatreRepertoireSyncService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMediator _mediator;
    private readonly ITheatreRepository _theatreRepository;
    private readonly ILogger<TheatreRepertoireSyncService> _logger;

    private const string ApiUrl = "https://www.teatrpolski.wroc.pl/api/repertoire?lang=pl";
    private const string TheatreName = "Teatr Polski we Wrocławiu";
    private const int TimeoutSeconds = 30;

    /// <summary>
    /// Initializes a new instance of the TheatreRepertoireSyncService
    /// </summary>
    public TheatreRepertoireSyncService(
        IHttpClientFactory httpClientFactory,
        IMediator mediator,
        ITheatreRepository theatreRepository,
        ILogger<TheatreRepertoireSyncService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _mediator = mediator;
        _theatreRepository = theatreRepository;
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

            // Get or create the theatre
            var theatre = await GetOrCreateTheatreAsync();
            if (theatre == null)
            {
                result.ErrorMessage = $"Failed to get or create theatre: {TheatreName}";
                _logger.LogError(result.ErrorMessage);
                return result;
            }

            // Fetch data from API
            var apiResponse = await FetchRepertoireDataAsync();
            if (apiResponse?.Events == null || apiResponse.Events.Count == 0)
            {
                _logger.LogWarning("API returned no events");
                result.TotalEventsProcessed = 0;
                return result;
            }

            _logger.LogInformation("Fetched {EventCount} events from API", apiResponse.Events.Count);
            result.TotalEventsProcessed = apiResponse.Events.Count;

            // Process each event and create/update shows
            foreach (var apiEvent in apiResponse.Events)
            {
                try
                {
                    // Skip hidden events
                    if (apiEvent.HiddenFromRepertoire)
                        continue;

                    // Convert API event to Show and save to database
                    var command = MapApiEventToCommand(apiEvent, theatre.Id);
                    if (command != null)
                    {
                        var commandResult = await _mediator.Send(command);
                        if (commandResult.IsSuccess)
                        {
                            result.SuccessCount++;
                        }
                        else
                        {
                            _logger.LogWarning("Command failed for event {EventId}: {Error}",
                                apiEvent.RepertoireEventId, commandResult.Error);
                            result.FailureCount++;
                        }
                    }
                    else
                    {
                        result.FailureCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process event {EventId}: {EventTitle}",
                        apiEvent.RepertoireEventId, apiEvent.Title);
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
    /// Get existing theatre by name or create a new one
    /// </summary>
    private async Task<Domain.Entities.Theatre?> GetOrCreateTheatreAsync()
    {
        try
        {
            var theatre = await _theatreRepository.GetByNameAsync(TheatreName);
            if (theatre != null)
            {
                _logger.LogDebug("Found existing theatre: {TheatreName}", TheatreName);
                return theatre;
            }

            _logger.LogInformation("Theatre not found, creating new one: {TheatreName}", TheatreName);
            var newTheatre = new Domain.Entities.Theatre
            {
                Name = TheatreName,
                City = "Wrocław",
                Address = "Wrocław, Poland",
                IsActive = true
            };

            await _theatreRepository.AddAsync(newTheatre);
            return newTheatre;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or creating theatre");
            return null;
        }
    }

    /// <summary>
    /// Fetch repertoire data from Theatre API with timeout and error handling
    /// </summary>
    private async Task<RepertoireApiResponse?> FetchRepertoireDataAsync()
    {
        using var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);

        try
        {
            _logger.LogDebug("Fetching data from {ApiUrl}", ApiUrl);
            var response = await client.GetAsync(ApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<RepertoireApiResponse>(json);
            _logger.LogInformation("Successfully fetched data from {ApiUrl}", ApiUrl);
            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching from {ApiUrl}", ApiUrl);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing API response");
            throw;
        }
    }

    /// <summary>
    /// Map API event to SaveOrUpdateShowCommand for database persistence
    /// Returns null if mapping cannot be completed
    /// </summary>
    private SaveOrUpdateShowCommand? MapApiEventToCommand(RepertoireEventDto apiEvent, Guid theatreId)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(apiEvent.Title) ||
                apiEvent.Stage?.Building == null)
            {
                _logger.LogWarning("Event {EventId} missing required fields", apiEvent.RepertoireEventId);
                return null;
            }

            var performanceType = ApiDtoMapper.DeterminePerformanceType(apiEvent);
            var ageRestriction = ApiDtoMapper.DetermineAgeRestriction(apiEvent);
            var additionalProps = ApiDtoMapper.ParseAdditionalProps(apiEvent.AdditionalProps);

            // Extract language information from additional props if available
            var language = additionalProps.ContainsKey("status_pl") &&
                additionalProps["status_pl"].Contains("English", StringComparison.OrdinalIgnoreCase)
                ? "English"
                : "Polish";

            _logger.LogDebug(
                "Mapped event {EventId}: Type={Type}, AgeRestriction={Age}, Language={Language}",
                apiEvent.RepertoireEventId, performanceType, ageRestriction, language);

            return new SaveOrUpdateShowCommand
            {
                Title = apiEvent.Title,
                ExternalId = apiEvent.RepertoireEventId,
                StartDateTime = apiEvent.Date,
                DurationMinutes = (int)apiEvent.Duration,
                PerformanceType = performanceType,
                AgeRestriction = ageRestriction,
                Language = language,
                TicketUrl = apiEvent.PaymentUrl,
                IsBookingDisabled = apiEvent.PaymentDisabled,
                TheatreId = theatreId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping event {EventId}", apiEvent.RepertoireEventId);
            return null;
        }
    }
}
