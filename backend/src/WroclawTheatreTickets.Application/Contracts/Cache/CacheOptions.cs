namespace WroclawTheatreTickets.Application.Contracts.Cache;

/// <summary>
/// Configuration options for cache behavior and TTLs.
/// Loaded from appsettings.json under "CacheOptions".
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// TTL for theatres cache in minutes (default: 1440 = 24 hours).
    /// </summary>
    public int TheatresTtlMinutes { get; set; } = 1440;

    /// <summary>
    /// TTL for all active shows cache in minutes (default: 15).
    /// </summary>
    public int AllShowsTtlMinutes { get; set; } = 15;

    /// <summary>
    /// TTL for upcoming shows cache in minutes (default: 30).
    /// </summary>
    public int UpcomingShowsTtlMinutes { get; set; } = 30;

    /// <summary>
    /// TTL for trending (most viewed) shows cache in minutes (default: 60).
    /// </summary>
    public int TrendingShowsTtlMinutes { get; set; } = 60;

    /// <summary>
    /// TTL for single show detail cache in minutes (default: 10).
    /// </summary>
    public int ShowDetailTtlMinutes { get; set; } = 10;

    /// <summary>
    /// TTL for show search results cache in minutes (default: 5).
    /// </summary>
    public int SearchResultsTtlMinutes { get; set; } = 5;

    /// <summary>
    /// TTL for filtered shows results cache in minutes (default: 10).
    /// </summary>
    public int FilteredShowsTtlMinutes { get; set; } = 10;

    /// <summary>
    /// TTL for approved reviews cache in minutes (default: 30).
    /// </summary>
    public int ReviewsTtlMinutes { get; set; } = 30;

    /// <summary>
    /// TTL for user favorites cache in minutes (default: 5).
    /// </summary>
    public int UserFavoritesTtlMinutes { get; set; } = 5;

    /// <summary>
    /// Whether caching is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Convert a TTL in minutes to TimeSpan for cache operations.
    /// </summary>
    public static TimeSpan ToTimeSpan(int minutes) => TimeSpan.FromMinutes(minutes);
}
