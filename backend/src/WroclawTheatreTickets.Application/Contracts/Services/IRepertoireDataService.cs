namespace WroclawTheatreTickets.Application.Contracts.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WroclawTheatreTickets.Domain.Entities;

/// <summary>
/// Service for fetching and mapping theatre repertoire data from external API.
/// Each implementation handles a specific theatre's API format and data structure.
/// </summary>
public interface IRepertoireDataService
{
    /// <summary>
    /// Fetches and maps theatre repertoire from API to Show domain entities.
    /// Handles validation, filtering of hidden events, and DTO to entity conversion.
    /// </summary>
    /// <param name="theatreId">ID of the theatre to associate with fetched shows</param>
    /// <param name="cancellationToken">Cancellation token for async operations</param>
    /// <returns>List of mapped Show entities ready for persistence. Empty list if no valid shows found.</returns>
    Task<List<Show>> FetchAndMapRepertoireAsync(Guid theatreId, CancellationToken cancellationToken);

    // Expose theatre-specific metadata so sync services can obtain name/city/address
    string TheatreName { get; }
    string City { get; }
    string Address { get; }
}
