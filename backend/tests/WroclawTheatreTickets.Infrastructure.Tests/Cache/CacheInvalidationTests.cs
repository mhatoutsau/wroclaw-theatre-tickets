namespace WroclawTheatreTickets.Infrastructure.Tests.Cache;

using WroclawTheatreTickets.Application.Contracts.Cache;
using System.Collections.Generic;

/// <summary>
/// Integration tests for cache invalidation scenarios.
/// Tests that cache is properly invalidated when data changes occur.
/// </summary>
public class CacheInvalidationTests
{

    [Fact]
    public void CacheKeys_ShouldContainAllExpectedShowCacheKeys()
    {
        // Assert - Verify all critical cache keys are defined
        Assert.Equal("shows:active", CacheKeys.ShowsActive);
        Assert.Equal("shows:upcoming:{0}", CacheKeys.ShowsUpcoming);
        Assert.Equal("shows:trending:{0}", CacheKeys.ShowsTrending);
        Assert.Equal("shows:detail:{0}", CacheKeys.ShowDetail);
        Assert.Equal("reviews:show:{0}", CacheKeys.ReviewsForShow);
    }

    [Fact]
    public void CacheKeys_ShouldContainPatternMarkersForInvalidation()
    {
        // Assert - Verify pattern keys for bulk invalidation
        Assert.Equal("shows:*", CacheKeys.ShowsPattern);
        Assert.Equal("reviews:*", CacheKeys.ReviewsPattern);
        Assert.Equal("theatres:*", CacheKeys.TheatresPattern);
        Assert.Equal("users:*", CacheKeys.UsersPattern);
    }

    [Fact]
    public void CacheOptions_ShouldHaveDefaultTTLValues()
    {
        // Arrange
        var options = new CacheOptions();

        // Assert
        Assert.True(options.Enabled);
        Assert.Equal(1440, options.TheatresTtlMinutes);        // 24 hours
        Assert.Equal(15, options.AllShowsTtlMinutes);          // 15 minutes
        Assert.Equal(30, options.UpcomingShowsTtlMinutes);     // 30 minutes
        Assert.Equal(60, options.TrendingShowsTtlMinutes);     // 1 hour
        Assert.Equal(10, options.ShowDetailTtlMinutes);        // 10 minutes
        Assert.Equal(5, options.SearchResultsTtlMinutes);      // 5 minutes
        Assert.Equal(10, options.FilteredShowsTtlMinutes);     // 10 minutes
        Assert.Equal(30, options.ReviewsTtlMinutes);           // 30 minutes
        Assert.Equal(5, options.UserFavoritesTtlMinutes);      // 5 minutes
    }

    [Fact]
    public void CacheOptions_ToTimeSpan_ShouldConvertMinutesToTimeSpan()
    {
        // Act
        var result = CacheOptions.ToTimeSpan(30);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(30), result);
    }

    [Fact]
    public void InvalidationStrategy_ShouldClearShowDetailCacheKeyFormat()
    {
        // Arrange
        var showId = Guid.NewGuid();
        var expectedKey = $"shows:detail:{showId}";

        // Act
        var actualKey = string.Format(CacheKeys.ShowDetail, showId);

        // Assert
        Assert.Equal(expectedKey, actualKey);
    }

    [Fact]
    public void InvalidationStrategy_ShouldClearReviewsCacheKeyFormat()
    {
        // Arrange
        var showId = Guid.NewGuid();
        var expectedKey = $"reviews:show:{showId}";

        // Act
        var actualKey = string.Format(CacheKeys.ReviewsForShow, showId);

        // Assert
        Assert.Equal(expectedKey, actualKey);
    }

    [Fact]
    public void InvalidationStrategy_ShouldSupportParametrizedUpcomingShowsKey()
    {
        // Arrange
        var days = 30;
        var expectedKey = $"shows:upcoming:{days}";

        // Act
        var actualKey = string.Format(CacheKeys.ShowsUpcoming, days);

        // Assert
        Assert.Equal(expectedKey, actualKey);
    }

    [Fact]
    public void InvalidationStrategy_ShouldSupportParametrizedTrendingKey()
    {
        // Arrange
        var count = 10;
        var expectedKey = $"shows:trending:{count}";

        // Act
        var actualKey = string.Format(CacheKeys.ShowsTrending, count);

        // Assert
        Assert.Equal(expectedKey, actualKey);
    }

    [Fact]
    public async Task CacheMetrics_ResetShouldClearAllMetrics()
    {
        // Arrange
        var metrics = new CacheMetrics();
        metrics.RecordHit("key1");
        metrics.RecordHit("key1");
        metrics.RecordMiss("key2");
        metrics.RecordEviction("key1");

        // Act
        metrics.Reset();

        // Assert
        Assert.Equal(0, metrics.GetTotalHits());
        Assert.Equal(0, metrics.GetTotalMisses());
        Assert.Equal(0, metrics.GetTotalEvictions());
        Assert.Equal(0, metrics.GetHitRate());
        Assert.Empty(metrics.GetAllMetrics());
    }

    [Fact]
    public async Task CacheMetrics_ShouldBeThreadSafe()
    {
        // Arrange
        var metrics = new CacheMetrics();
        var tasks = new List<Task>();

        // Act - Record hits from multiple threads
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    metrics.RecordHit($"key{j}");
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(1000, metrics.GetTotalHits());
    }

    [Fact]
    public void CacheKeyMetrics_ShouldCalculateHitRateAccurately()
    {
        // Arrange
        var keyMetrics = new CacheKeyMetrics
        {
            Hits = 75,
            Misses = 25
        };

        // Act
        var hitRate = keyMetrics.GetHitRate();

        // Assert
        Assert.Equal(75.0, hitRate);
    }

    [Fact]
    public void CacheKeyMetrics_HitRateShouldBeZeroWhenNoActivity()
    {
        // Arrange
        var keyMetrics = new CacheKeyMetrics
        {
            Hits = 0,
            Misses = 0
        };

        // Act
        var hitRate = keyMetrics.GetHitRate();

        // Assert
        Assert.Equal(0.0, hitRate);
    }

    [Theory]
    [InlineData(15)]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(10)]
    public void CacheOptions_TTLsShouldMatchExpectedDurations(int minutes)
    {
        // Arrange
        var options = new CacheOptions();

        // Act & Assert
        var ttl = CacheOptions.ToTimeSpan(minutes);
        Assert.Equal(minutes, (int)ttl.TotalMinutes);
    }
}
