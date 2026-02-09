namespace WroclawTheatreTickets.Application.Contracts.Cache;

using System.Collections.Concurrent;

/// <summary>
/// Tracks cache performance metrics (hits, misses, evictions).
/// Thread-safe for concurrent access patterns.
/// </summary>
public class CacheMetrics
{
    private readonly ConcurrentDictionary<string, CacheKeyMetrics> _keyMetrics;
    private long _totalHits;
    private long _totalMisses;
    private long _totalEvictions;

    public CacheMetrics()
    {
        _keyMetrics = new ConcurrentDictionary<string, CacheKeyMetrics>();
        _totalHits = 0;
        _totalMisses = 0;
        _totalEvictions = 0;
    }

    /// <summary>
    /// Records a cache hit for the specified key.
    /// </summary>
    public void RecordHit(string key)
    {
        Interlocked.Increment(ref _totalHits);
        var metrics = _keyMetrics.AddOrUpdate(
            key,
            new CacheKeyMetrics(),
            (_, existing) => { existing.Hits++; return existing; });
        metrics.Hits++;
    }

    /// <summary>
    /// Records a cache miss for the specified key.
    /// </summary>
    public void RecordMiss(string key)
    {
        Interlocked.Increment(ref _totalMisses);
        var metrics = _keyMetrics.AddOrUpdate(
            key,
            new CacheKeyMetrics(),
            (_, existing) => { existing.Misses++; return existing; });
        metrics.Misses++;
    }

    /// <summary>
    /// Records a cache eviction for the specified key.
    /// </summary>
    public void RecordEviction(string key)
    {
        Interlocked.Increment(ref _totalEvictions);
        if (_keyMetrics.TryGetValue(key, out var metrics))
        {
            metrics.Evictions++;
        }
    }

    /// <summary>
    /// Gets the overall cache hit rate as a percentage (0-100).
    /// Returns 0 if no cache activity has occurred.
    /// </summary>
    public double GetHitRate()
    {
        var total = _totalHits + _totalMisses;
        return total == 0 ? 0 : (_totalHits * 100.0) / total;
    }

    /// <summary>
    /// Gets the total number of hits.
    /// </summary>
    public long GetTotalHits()
    {
        return _totalHits;
    }

    /// <summary>
    /// Gets the total number of misses.
    /// </summary>
    public long GetTotalMisses()
    {
        return _totalMisses;
    }

    /// <summary>
    /// Gets the total number of evictions.
    /// </summary>
    public long GetTotalEvictions()
    {
        return _totalEvictions;
    }

    /// <summary>
    /// Gets top keys by hit count.
    /// </summary>
    public IEnumerable<(string Key, long Hits)> GetTopKeysByHits(int count = 10)
    {
        return _keyMetrics
            .OrderByDescending(kvp => kvp.Value.Hits)
            .Take(count)
            .Select(kvp => (kvp.Key, kvp.Value.Hits));
    }

    /// <summary>
    /// Gets all tracked metrics.
    /// </summary>
    public IDictionary<string, CacheKeyMetrics> GetAllMetrics()
    {
        return new Dictionary<string, CacheKeyMetrics>(_keyMetrics);
    }

    /// <summary>
    /// Resets all metrics to zero.
    /// </summary>
    public void Reset()
    {
        _keyMetrics.Clear();
        _totalHits = 0;
        _totalMisses = 0;
        _totalEvictions = 0;
    }
}

/// <summary>
/// Metrics for a single cache key.
/// </summary>
public class CacheKeyMetrics
{
    public long Hits { get; set; }
    public long Misses { get; set; }
    public long Evictions { get; set; }

    public double GetHitRate()
    {
        var total = Hits + Misses;
        return total == 0 ? 0 : (Hits * 100.0) / total;
    }
}
