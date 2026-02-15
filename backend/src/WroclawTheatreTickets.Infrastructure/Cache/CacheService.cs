namespace WroclawTheatreTickets.Infrastructure.Cache;

using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Serilog;
using WroclawTheatreTickets.Application.Contracts.Cache;
using WroclawTheatreTickets.Application.Contracts.Services;

/// <summary>
/// Implementation of ICacheService using IDistributedCache with JSON serialization, metrics tracking,
/// and pattern-based removal support. Supports both in-memory and Redis backends
/// without handler changes.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly CacheMetrics _metrics;
    private static readonly ILogger _logger = Log.ForContext<CacheService>();

    public CacheService(IDistributedCache distributedCache, CacheMetrics metrics)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await _distributedCache.GetStringAsync(key);
            
            if (json == null)
            {
                _metrics.RecordMiss(key);
                _logger.Debug("Cache miss for key: {CacheKey}", key);
                return default;
            }

            var value = JsonSerializer.Deserialize<T>(json);
            _metrics.RecordHit(key);
            _logger.Debug("Cache hit for key: {CacheKey}, type: {Type}", key, typeof(T).Name);
            
            return value;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving cache for key: {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }

            await _distributedCache.SetStringAsync(key, json, options);
            _logger.Debug("Cache set for key: {CacheKey}, type: {Type}, expiration: {Expiration}", 
                key, typeof(T).Name, expiration?.TotalSeconds ?? -1);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error setting cache for key: {CacheKey}", key);
            // Don't throw - cache failures shouldn't break the application
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _distributedCache.RemoveAsync(key);
            _metrics.RecordEviction(key);
            _logger.Debug("Cache invalidated for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error removing cache for key: {CacheKey}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            // Note: IDistributedCache doesn't natively support pattern matching.
            // For in-memory cache (MemoryCache), we'd need a custom wrapper.
            // For Redis, this would use SCAN with pattern.
            // Currently, this is a placeholder that logs the pattern removal intention.
            // A full implementation would require either:
            // 1. Custom MemoryCache wrapper tracking all keys
            // 2. Redis-specific implementation
            // For now, handlers will call RemoveAsync with explicit keys.
            
            _logger.Information("Pattern removal requested for pattern: {CachePattern} - implement backend-specific logic", pattern);
            
            // This method will be enhanced based on the chosen backend
            // For in-memory cache: override in a custom implementation
            // For Redis: use Redis SCAN + pattern matching
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error removing cache by pattern: {CachePattern}", pattern);
        }
    }

    /// <summary>
    /// Gets the current cache metrics (hits, misses, etc.).
    /// </summary>
    public CacheMetrics GetMetrics()
    {
        return _metrics;
    }
}
