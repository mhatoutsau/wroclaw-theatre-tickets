namespace WroclawTheatreTickets.Application.Contracts.Services;

using WroclawTheatreTickets.Domain.Entities;

/// <summary>
/// Service for managing theatre entity lifecycle - lookup and creation.
/// Provides a single theatre entity per synchronization job.
/// </summary>
public interface ITheatreProviderService
{
    /// <summary>
    /// Gets an existing theatre by name or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="theatreName">Name of the theatre</param>
    /// <param name="city">City where the theatre is located</param>
    /// <param name="address">Full address of the theatre</param>
    /// <returns>Existing or newly created Theatre entity, or null if an error occurred</returns>
    Task<Theatre?> GetOrCreateTheatreAsync(string theatreName, string city, string address);
}
