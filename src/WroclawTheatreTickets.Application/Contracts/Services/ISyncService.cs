namespace WroclawTheatreTickets.Application.Contracts.Services;

/// <summary>
/// Service contract for synchronizing theatre repertoire data from external sources
/// </summary>
public interface ITheatreRepertoireSyncService
{
    /// <summary>
    /// Synchronizes theatre repertoire from the Theatre API to the local database
    /// </summary>
    /// <returns>Result containing success count and failure count</returns>
    Task<TheatreSyncResult> SyncTheatreRepertoireAsync();
}

/// <summary>
/// Result of a theatre synchronization operation
/// </summary>
public class TheatreSyncResult
{
    /// <summary>
    /// Number of shows successfully created or updated
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of shows that failed to process
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// Total number of events fetched from API
    /// </summary>
    public int TotalEventsProcessed { get; set; }

    /// <summary>
    /// Error message if synchronization failed completely
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Whether the synchronization completed successfully
    /// </summary>
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
}
