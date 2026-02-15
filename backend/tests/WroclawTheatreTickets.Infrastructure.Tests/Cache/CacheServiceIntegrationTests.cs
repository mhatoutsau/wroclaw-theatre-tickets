namespace WroclawTheatreTickets.Infrastructure.Tests.Cache;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WroclawTheatreTickets.Application.Contracts.Cache;
using WroclawTheatreTickets.Infrastructure.Cache;
using System.Text.Json;

/// <summary>
/// Integration tests for CacheService with in-memory distributed cache backend.
/// </summary>
public class CacheServiceIntegrationTests
{
    private readonly IDistributedCache _cache;
    private readonly CacheMetrics _metrics;
    private readonly CacheService _cacheService;

    public CacheServiceIntegrationTests()
    {
        _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        _metrics = new CacheMetrics();
        _cacheService = new CacheService(_cache, _metrics);
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_ShouldReturnCachedValue()
    {
        // Arrange
        var key = "test:key:1";
        var value = new TestModel { Id = 1, Name = "Test Item" };

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<TestModel>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(value.Id, result.Id);
        Assert.Equal(value.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_ForNonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var key = "non:existent:key";

        // Act
        var result = await _cacheService.GetAsync<TestModel>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveCachedValue()
    {
        // Arrange
        var key = "test:remove:key";
        var value = new TestModel { Id = 2, Name = "Remove Test" };
        await _cacheService.SetAsync(key, value);

        // Act
        await _cacheService.RemoveAsync(key);
        var result = await _cacheService.GetAsync<TestModel>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_ShouldExpireAfterTimespan()
    {
        // Arrange
        var key = "test:expiration:key";
        var value = new TestModel { Id = 3, Name = "Expiration Test" };
        var expiration = TimeSpan.FromMilliseconds(100);

        // Act
        await _cacheService.SetAsync(key, value, expiration);
        var resultBefore = await _cacheService.GetAsync<TestModel>(key);
        
        await Task.Delay(150); // Wait for expiration
        var resultAfter = await _cacheService.GetAsync<TestModel>(key);

        // Assert
        Assert.NotNull(resultBefore);
        Assert.Null(resultAfter);
    }

    [Fact]
    public async Task SetAsync_MultipleTypes_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var stringKey = "test:string";
        var stringValue = "Test String Value";
        
        var intKey = "test:int";
        var intValue = 42;
        
        var listKey = "test:list";
        var listValue = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        await _cacheService.SetAsync(stringKey, stringValue);
        await _cacheService.SetAsync(intKey, intValue);
        await _cacheService.SetAsync(listKey, listValue);

        var stringResult = await _cacheService.GetAsync<string>(stringKey);
        var intResult = await _cacheService.GetAsync<int>(intKey);
        var listResult = await _cacheService.GetAsync<List<int>>(listKey);

        // Assert
        Assert.Equal(stringValue, stringResult);
        Assert.Equal(intValue, intResult);
        Assert.Equal(listValue, listResult);
    }

    [Fact]
    public async Task RecordHit_ShouldIncrementHitMetrics()
    {
        // Arrange
        var key = "test:metrics:hit";
        var value = new TestModel { Id = 4, Name = "Metrics Test" };
        await _cacheService.SetAsync(key, value);

        // Act
        var result1 = await _cacheService.GetAsync<TestModel>(key);
        var result2 = await _cacheService.GetAsync<TestModel>(key);
        var result3 = await _cacheService.GetAsync<TestModel>(key);

        var metrics = _cacheService.GetMetrics();

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.Equal(3, metrics.GetTotalHits());
    }

    [Fact]
    public async Task RecordMiss_ShouldIncrementMissMetrics()
    {
        // Arrange
        var hitKey = "test:hit";
        var missKey1 = "test:miss:1";
        var missKey2 = "test:miss:2";
        
        await _cacheService.SetAsync(hitKey, new TestModel { Id = 1 });

        // Act
        await _cacheService.GetAsync<TestModel>(hitKey);      // 1 hit
        await _cacheService.GetAsync<TestModel>(missKey1);    // 1 miss
        await _cacheService.GetAsync<TestModel>(missKey2);    // 1 miss

        var metrics = _cacheService.GetMetrics();

        // Assert
        Assert.Equal(1, metrics.GetTotalHits());
        Assert.Equal(2, metrics.GetTotalMisses());
        Assert.InRange(metrics.GetHitRate(), 33.0, 34.0);  // About 33.33%
    }

    [Fact]
    public async Task RecordEviction_ShouldTrackRemovals()
    {
        // Arrange
        var key = "test:eviction";
        var value = new TestModel { Id = 5, Name = "Eviction Test" };
        await _cacheService.SetAsync(key, value);

        // Act
        await _cacheService.RemoveAsync(key);
        var metrics = _cacheService.GetMetrics();

        // Assert
        Assert.Equal(1, metrics.GetTotalEvictions());
    }

    [Fact]
    public async Task GetHitRate_ShouldCalculateCorrectPercentage()
    {
        // Arrange - 10 hits, 5 misses = 66.67%
        await _cacheService.SetAsync("hit", new TestModel { Id = 1 });

        for (int i = 0; i < 10; i++)
        {
            await _cacheService.GetAsync<TestModel>("hit");
        }

        for (int i = 0; i < 5; i++)
        {
            await _cacheService.GetAsync<TestModel>("miss");
        }

        // Act
        var hitRate = _cacheService.GetMetrics().GetHitRate();

        // Assert
        Assert.InRange(hitRate, 66.0, 67.0);  // About 66.67%
    }

    [Fact]
    public async Task Cache_ShouldHandleEmptyStringValues()
    {
        // Arrange
        var key = "test:empty:string";
        var value = "";

        // Act
        await _cacheService.SetAsync(key, value);
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("", result);
    }

    [Fact]
    public async Task Cache_ShouldHandleNullObjects()
    {
        // Arrange
        var key = "test:null:object";
        TestModel? value = null;

        // Act & Assert
        // Setting null should not throw
        await _cacheService.SetAsync(key, value!);
    }

    /// <summary>
    /// Test model for cache testing.
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
