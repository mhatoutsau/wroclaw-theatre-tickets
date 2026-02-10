namespace WroclawTheatreTickets.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Configuration;

/// <summary>
/// Service for fetching and mapping Teatr Polski repertoire data from external API.
/// Handles HTTP communication, JSON deserialization, validation, and mapping to Show domain entities.
/// </summary>
public class TeatrPolskiRepertoireDataService : IRepertoireDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<TheatreApiConfiguration> _config;
    private readonly ILogger<TeatrPolskiRepertoireDataService> _logger;

    // Expose theatre-specific metadata
    public string TheatreName => "Teatr Polski we Wrocławiu";
    public string City => "Wrocław";
    public string Address => "Wrocław, Poland";

    /// <summary>
    /// Initializes a new instance of TeatrPolskiRepertoireDataService
    /// </summary>
    public TeatrPolskiRepertoireDataService(
        IHttpClientFactory httpClientFactory,
        IOptions<TheatreApiConfiguration> config,
        ILogger<TeatrPolskiRepertoireDataService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Fetches and maps theatre repertoire from Teatr Polski API to Show domain entities.
    /// Handles validation, filtering of hidden events, and DTO to entity conversion.
    /// </summary>
    public async Task<List<Show>> FetchAndMapRepertoireAsync(Guid theatreId, CancellationToken cancellationToken)
    {
        var results = new List<Show>();

        try
        {
            _logger.LogDebug("Fetching Teatr Polski repertoire for theatre {TheatreId}", theatreId);

            // Fetch data from API
            var apiResponse = await FetchRepertoireDataAsync(cancellationToken);
            if (apiResponse?.Events == null || apiResponse.Events.Count == 0)
            {
                _logger.LogWarning("Teatr Polski API returned no events");
                return results;
            }

            _logger.LogInformation("Fetched {EventCount} events from Teatr Polski API", apiResponse.Events.Count);

            // Process and map each event
            foreach (var apiEvent in apiResponse.Events)
            {
                try
                {
                    // Skip hidden events
                    if (apiEvent.HiddenFromRepertoire)
                    {
                        _logger.LogDebug("Skipping hidden event {EventId}: {Title}", 
                            apiEvent.RepertoireEventId, apiEvent.Title);
                        continue;
                    }

                    // Map API event to Show entity
                    var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(apiEvent, theatreId);
                    if (show != null)
                    {
                        results.Add(show);
                        _logger.LogDebug(
                            "Mapped event {EventId}: Type={Type}, AgeRestriction={Age}, Language={Language}",
                            apiEvent.RepertoireEventId, show.Type, show.AgeRestriction, show.Language);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to map event {EventId}: missing required fields", 
                            apiEvent.RepertoireEventId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error mapping event {EventId}: {Title}",
                        apiEvent.RepertoireEventId, apiEvent.Title);
                }
            }

            _logger.LogInformation(
                "Successfully mapped {MappedCount} of {TotalCount} events from Teatr Polski",
                results.Count, apiResponse.Events.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching and mapping Teatr Polski repertoire");
            throw;
        }
    }

    /// <summary>
    /// Fetch repertoire data from Teatr Polski API with timeout and error handling
    /// </summary>
    private async Task<RepertoireApiResponse?> FetchRepertoireDataAsync(CancellationToken cancellationToken)
    {
        using var client = _httpClientFactory.CreateClient("TeatrPolski");
        client.Timeout = TimeSpan.FromSeconds(_config.Value.TimeoutSeconds);

        try
        {
            _logger.LogDebug("Fetching data from Teatr Polski API: {Url}", _config.Value.Url);
            
            var response = await client.GetAsync(_config.Value.Url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = System.Text.Json.JsonSerializer.Deserialize<RepertoireApiResponse>(json);
            _logger.LogInformation("Successfully fetched data from Teatr Polski API");
            
            return apiResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching from Teatr Polski API: {Url}", _config.Value.Url);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Request to Teatr Polski API timed out after {TimeoutSeconds} seconds", 
                _config.Value.TimeoutSeconds);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Teatr Polski API response");
            throw;
        }
    }
}
