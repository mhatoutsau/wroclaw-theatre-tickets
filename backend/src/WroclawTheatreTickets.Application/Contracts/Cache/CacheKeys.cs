namespace WroclawTheatreTickets.Application.Contracts.Cache;

/// <summary>
/// Constants for cache keys and patterns used throughout the application.
/// Enables centralized management of cache key naming and invalidation patterns.
/// </summary>
public static class CacheKeys
{
    // =====================================================
    // Theatre Cache Keys
    // =====================================================
    
    /// <summary>
    /// All active theatres. TTL: 24 hours (rarely changes).
    /// Pattern: theatres:all
    /// </summary>
    public const string TheatresAll = "theatres:all";

    /// <summary>
    /// Pattern for theatre-related caches.
    /// Used for bulk invalidation of theatre caches.
    /// </summary>
    public const string TheatresPattern = "theatres:*";

    // =====================================================
    // Show Cache Keys
    // =====================================================

    /// <summary>
    /// All active shows. TTL: 15 minutes.
    /// Pattern: shows:active
    /// </summary>
    public const string ShowsActive = "shows:active";

    /// <summary>
    /// Upcoming shows (within specified days). TTL: 30 minutes.
    /// Pattern: shows:upcoming:{days}
    /// </summary>
    public const string ShowsUpcoming = "shows:upcoming:{0}";

    /// <summary>
    /// Most viewed shows. TTL: 1 hour.
    /// Pattern: shows:trending:{count}
    /// </summary>
    public const string ShowsTrending = "shows:trending:{0}";

    /// <summary>
    /// Single show detail with reviews. TTL: 10 minutes.
    /// Pattern: shows:detail:{showId}
    /// </summary>
    public const string ShowDetail = "shows:detail:{0}";

    /// <summary>
    /// Show search results. TTL: 5 minutes.
    /// Pattern: shows:search:{keyword}
    /// </summary>
    public const string ShowsSearch = "shows:search:{0}";

    /// <summary>
    /// Filtered shows results. TTL: 10 minutes.
    /// Pattern: shows:filtered:{filterHash}
    /// </summary>
    public const string ShowsFiltered = "shows:filtered:{0}";

    /// <summary>
    /// Pattern for all show-related caches.
    /// Used for bulk invalidation when shows are modified.
    /// </summary>
    public const string ShowsPattern = "shows:*";

    // =====================================================
    // Review Cache Keys
    // =====================================================

    /// <summary>
    /// Approved reviews for a show. TTL: 30 minutes.
    /// Pattern: reviews:show:{showId}
    /// </summary>
    public const string ReviewsForShow = "reviews:show:{0}";

    /// <summary>
    /// Pattern for review-related caches.
    /// Used for bulk invalidation when reviews are changed.
    /// </summary>
    public const string ReviewsPattern = "reviews:*";

    // =====================================================
    // User Cache Keys
    // =====================================================

    /// <summary>
    /// User favorites. TTL: 5 minutes.
    /// Pattern: users:favorites:{userId}
    /// </summary>
    public const string UserFavorites = "users:favorites:{0}";

    /// <summary>
    /// Pattern for user-related caches.
    /// Used for bulk invalidation when user data changes.
    /// </summary>
    public const string UsersPattern = "users:*";

    // =====================================================
    // Cache Configuration
    // =====================================================

    /// <summary>
    /// Configuration section name for cache options in appsettings.json
    /// </summary>
    public const string ConfigurationSection = "CacheOptions";
}
