namespace WroclawTheatreTickets.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Domain.Entities;

/// <summary>
/// Service for managing theatre entity lifecycle - lookup and creation.
/// Provides a single theatre entity per synchronization job.
/// </summary>
public class TheatreProviderService : ITheatreProviderService
{
    private readonly ITheatreRepository _theatreRepository;
    private readonly ILogger<TheatreProviderService> _logger;

    /// <summary>
    /// Initializes a new instance of TheatreProviderService
    /// </summary>
    public TheatreProviderService(
        ITheatreRepository theatreRepository,
        ILogger<TheatreProviderService> logger)
    {
        _theatreRepository = theatreRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets an existing theatre by name or creates a new one if it doesn't exist.
    /// </summary>
    public async Task<Theatre?> GetOrCreateTheatreAsync(string theatreName, string city, string address)
    {
        try
        {
            // Try to find existing theatre
            var existingTheatre = await _theatreRepository.GetByNameAsync(theatreName);
            if (existingTheatre != null)
            {
                _logger.LogDebug("Found existing theatre: {TheatreName}", theatreName);
                return existingTheatre;
            }

            // Create new theatre if not found using the static Create method
            _logger.LogInformation("Theatre not found, creating new one: {TheatreName}", theatreName);
            var newTheatre = Theatre.Create(theatreName, address);
            newTheatre.City = city;

            await _theatreRepository.AddAsync(newTheatre);
            _logger.LogInformation("Successfully created new theatre: {TheatreName} (ID: {TheatreId})", 
                theatreName, newTheatre.Id);

            return newTheatre;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting or creating theatre: {TheatreName}", theatreName);
            return null;
        }
    }
}
