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
