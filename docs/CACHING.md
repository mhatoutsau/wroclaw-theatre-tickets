# Caching Layer Guide - Wrocław Theatre Tickets

## Overview

The caching layer uses .NET's `IDistributedCache` abstraction to provide a flexible, performant caching solution. The implementation supports:
- **In-memory caching** (Phase 1, current)
- **Redis caching** (Phase 2, future - no code changes needed)

## Quick Start

### Health Check Endpoint
```bash
curl http://localhost:5000/health/cache
```
Response includes cache hit rate, total hits/misses, and top accessed cache keys.

### Disabling Cache
Edit `appsettings.json`:
```json
"CacheOptions": {
  "Enabled": false
}
```

## Architecture

### Cache Service
Located: `WroclawTheatreTickets.Infrastructure.Cache.CacheService`

Implements `IDistributedCache` wrapper with:
- **JSON Serialization**: Automatic serialization/deserialization of complex types
- **Metrics Tracking**: Hits, misses, evictions per cache key
- **TTL Support**: Configurable expiration per cache type
- **Error Handling**: Failures don't crash the application; logs are generated

```csharp
var result = await _cacheService.GetAsync<ShowDto>("shows:detail:{showId}");
await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(10));
await _cacheService.RemoveAsync(key);
```

### Cache Keys
Located: `WroclawTheatreTickets.Application.Contracts.Cache.CacheKeys`

**Theatre Caches**
- `theatres:all` - All active theatres (24h TTL)

**Show Caches**
- `shows:active` - All active shows (15min TTL)
- `shows:upcoming:{days}` - Upcoming shows within N days (30min TTL)
- `shows:trending:{count}` - Most viewed shows (1h TTL)
- `shows:detail:{showId}` - Single show with reviews (10min TTL)
- `shows:search:{keyword}` - Search results (5min TTL)
- `shows:filtered:{filterHash}` - Filtered results (10min TTL)

**Review Caches**
- `reviews:show:{showId}` - Approved reviews for show (30min TTL)

**User Caches**
- `users:favorites:{userId}` - User's favorite shows (5min TTL)

**Invalidation Patterns**
- `shows:*` - All show-related caches
- `reviews:*` - All review-related caches
- `theatres:*` - All theatre-related caches
- `users:*` - All user-related caches

### Cache Configuration
Located: `WroclawTheatreTickets.Application.Contracts.Cache.CacheOptions`

Edit in `appsettings.json`:
```json
"CacheOptions": {
  "Enabled": true,
  "TheatresTtlMinutes": 1440,
  "AllShowsTtlMinutes": 15,
  "UpcomingShowsTtlMinutes": 30,
  "TrendingShowsTtlMinutes": 60,
  "ShowDetailTtlMinutes": 10,
  "SearchResultsTtlMinutes": 5,
  "FilteredShowsTtlMinutes": 10,
  "ReviewsTtlMinutes": 30,
  "UserFavoritesTtlMinutes": 5
}
```

### Cache Metrics
Located: `WroclawTheatreTickets.Application.Contracts.Cache.CacheMetrics`

Tracks:
- **Total Hits**: Successful cache retrievals
- **Total Misses**: Failed cache lookups
- **Total Evictions**: Manual cache removals
- **Hit Rate**: `(Hits / (Hits + Misses)) * 100%`
- **Top Keys**: Most frequently accessed cache keys

Access via:
```csharp
var metrics = _cacheService.GetMetrics();
var hitRate = metrics.GetHitRate();
var topKeys = metrics.GetTopKeysByHits(10);
```

## Cached Query Handlers

### GetAllShowsQuery
- **Cache Key**: `shows:active`
- **TTL**: 15 minutes
- **Hit Condition**: Request repeated within 15 minutes
- **Invalidation**: When any show is created/updated

### GetUpcomingShowsQuery
- **Cache Key**: `shows:upcoming:{days}`
- **TTL**: 30 minutes
- **Example**: `shows:upcoming:30` for 30-day window
- **Invalidation**: When shows are created/updated

### GetMostViewedShowsQuery
- **Cache Key**: `shows:trending:{count}`
- **TTL**: 1 hour
- **Example**: `shows:trending:10` for top 10
- **Invalidation**: When any show's view count changes (rare)

### GetShowByIdQuery
- **Cache Key**: `shows:detail:{showId}`
- **TTL**: 10 minutes
- **Invalidation**: When show is updated OR review is approved
- **Note**: View count increment bypasses cache (single-instance)

## Cache Invalidation Strategy

### Hybrid Approach: Eager + TTL

**Eager Invalidation** (Immediate)
```
Data Change Event
    ↓
Call CacheService.RemoveAsync(key)
    ↓
Cache entry removed immediately
```

**TTL Fallback** (Safety Net)
```
Cache Entry Created
    ↓
Wait for TTL expiration
    ↓
Automatic removal
```

### Invalidation Triggers

| Operation | Cache Keys Cleared | Reason |
|-----------|-------------------|--------|
| Show Create/Update | `shows:active`, `shows:upcoming:*`, `shows:trending:*`, `shows:detail:{id}`, `reviews:show:{id}` | Show data changed |
| Review Create | `shows:detail:{showId}`, `reviews:show:{showId}` | Review count/rating updated |
| Review Approve | `shows:detail:{showId}`, `reviews:show:{showId}` | Approved review visible |
| Review Remove | `shows:detail:{showId}`, `reviews:show:{showId}` | Approved reviews list changed |

## Performance Impact

### Expected Improvements
- **Database Load**: 60-80% reduction in queries for read-heavy operations
- **Response Time**: 2-5x faster for cached responses
- **Throughput**: Support 3-5x more concurrent users
- **API Latency**: Cache hits: <5ms vs Database queries: 50-500ms

### Cache Hit Scenarios
1. **Homepage**: Show listings (high hit rate in first 15 minutes)
2. **Browse Shows**: Upcoming shows for calendar (high repeat requests)
3. **Trending Section**: Most viewed shows (sustained high hits)
4. **Show Details**: Same user revisiting show multiple times (medium hits)

## Testing

### Integration Tests
Location: `tests/WroclawTheatreTickets.Infrastructure.Tests/Cache/`

**CacheServiceIntegrationTests**
- Cache set/get/remove operations
- Serialization of multiple types
- TTL expiration verification
- Metrics tracking accuracy
- Thread safety

**CacheInvalidationTests**
- Cache key format validation
- TTL configuration defaults
- Cache pattern matching
- Metrics reset operations

### Application Tests
Location: `tests/WroclawTheatreTickets.Application.Tests/UseCases/Shows/`

**CachedQueryHandlerTests**
- Cache hit scenarios (mocked)
- Cache miss fallback to repository
- Proper TTL usage per handler
- Disabled cache behavior
- Configuration-based TTL changes

### Run Tests
```bash
# All cache tests
dotnet test --filter "Category=Cache or FullyQualifiedName~CacheService"

# Infrastructure cache tests only
dotnet test tests/WroclawTheatreTickets.Infrastructure.Tests

# Application handler tests
dotnet test tests/WroclawTheatreTickets.Application.Tests
```

## Monitoring

### Health Endpoint
```bash
curl http://localhost:5000/health/cache
```

Response:
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-09T14:30:12Z",
  "hitRate": 72.5,
  "totalHits": 1450,
  "totalMisses": 550,
  "totalEvictions": 8,
  "topKeys": [
    { "key": "shows:active", "hits": 450 },
    { "key": "shows:detail:xyz", "hits": 380 },
    { "key": "shows:upcoming:30", "hits": 220 }
  ]
}
```

### Logging
Cache operations are logged at Debug level (disabled in production by default):
```
Cache miss for key: shows:active
Cache hit for key: shows:active, type: IEnumerable`1
Cache set for key: shows:active, type: IEnumerable`1, expiration: 900
Cache invalidated for key: shows:detail:xyz
```

Enable debug logging in `appsettings.Development.json`:
```json
"Logging": {
  "LogLevel": {
    "WroclawTheatreTickets.Infrastructure.Cache": "Debug"
  }
}
```

## Migration: In-Memory to Redis

### Phase 2: Redis Backend Setup

**No code changes required.** Only dependency injection configuration changes:

**Current (In-Memory)**
```csharp
services.AddDistributedMemoryCache();
```

**Future (Redis)**
```csharp
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis") 
        ?? "localhost:6379";
});
```

Add to `appsettings.json`:
```json
"ConnectionStrings": {
  "Redis": "redis-server.example.com:6379"
}
```

**Benefits**:
- Multi-instance scalability
- Persistent cache across deployments
- Automatic cache warming on restart
- Better memory efficiency at scale

## Best Practices

### 1. Cache Key Naming
✅ Use constants from `CacheKeys` class
❌ Don't hardcode cache keys

```csharp
// Good
var key = string.Format(CacheKeys.ShowDetail, showId);

// Bad
var key = $"show-detail-{showId}";
```

### 2. Cache Invalidation
✅ Invalidate multiple keys when related data changes
❌ Forget to invalidate dependent caches

```csharp
// Good: Clear show detail AND reviews when show updates
await _cacheService.RemoveAsync(string.Format(CacheKeys.ShowDetail, id));
await _cacheService.RemoveAsync(string.Format(CacheKeys.ReviewsForShow, id));

// Bad: Only clear one
await _cacheService.RemoveAsync(string.Format(CacheKeys.ShowDetail, id));
```

### 3. TTL Configuration
✅ Use configuration values from `CacheOptions`
❌ Hardcode TTLs in handlers

```csharp
// Good
var ttl = CacheOptions.ToTimeSpan(_cacheOptions.AllShowsTtlMinutes);
await _cacheService.SetAsync(key, value, ttl);

// Bad
await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(15));
```

### 4. Error Handling
Cache failures are non-fatal:
- If cache write fails, request completes successfully
- If cache read fails, falls back to database
- All errors are logged

No need for manual exception handling around cache calls.

### 5. Testing
Always mock `ICacheService` in unit tests:
```csharp
var cacheMock = new Mock<ICacheService>();
cacheMock
    .Setup(c => c.GetAsync<T>(key))
    .ReturnsAsync(value);
```

## Troubleshooting

### Cache Not Working
1. Check `CacheOptions.Enabled` is `true`
2. Verify cache key format matches constants
3. Check TTL is appropriate (too short?)
4. Monitor `/health/cache` for hit rate

### High Memory Usage (In-Memory)
- Reduce TTLs in `CacheOptions`
- Disable cache for less critical queries
- Plan Redis migration (Phase 2)

### Stale Data
- Reduce TTL for that cache type
- Verify invalidation is triggered on data changes
- Check logs for invalidation errors

### Cache Not Invalidating
- Verify command handler calls `RemoveAsync`
- Check cache key format is correct
- Ensure cache is enabled in configuration
- Review logs for invalidation calls

## Performance Tuning

### TTL Adjustment
Monitor `/health/cache` hit rate and adjust:
- **High hit rate (>80%)**: TTL is optimal
- **Medium hit rate (50-80%)**: Consider increasing TTL slightly
- **Low hit rate (<50%)**: Either:
  - Decrease TTL (data changes frequently)
  - Disable cache (not read-heavy enough)

### Cache Key Design
- Use parametrized keys for flexible queries
- Keep keys short but descriptive
- Follow naming convention: `domain:entity:parameter`

### Startup Performance
- Cache isn't pre-warmed (lazy on first request)
- Future: Implement startup cache warming for critical data

## References

- [Microsoft.Extensions.Caching documentation](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed)
- [Redis Stack Exchange documentation](https://stackexchange.github.io/StackExchange.Redis/)
- ADR-021: Distributed Caching Layer Architecture Decision

---

**Document Version**: 1.0  
**Last Updated**: February 9, 2026  
**Next Review**: After performance metrics collection (Phase 1 completion)
