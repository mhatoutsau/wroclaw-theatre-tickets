namespace WroclawTheatreTickets.Application.Tests.UseCases.Shows;

using WroclawTheatreTickets.Application.Contracts.Cache;
using Microsoft.Extensions.Options;

/// <summary>
/// Tests for cached query handler configuration and cache key strategies.
/// </summary>
public class CachedQueryHandlerTests
{
    [Fact]
    public void CacheOptionsConfiguration_ShouldBeEnabledByDefault()
    {
        // Arrange
        var options = Options.Create(new CacheOptions());

        // Act & Assert
        Assert.True(options.Value.Enabled);
    }

    [Fact]
    public void CacheOptionsConfiguration_ShouldRespectCustomTTL()
    {
        // Arrange
        var customOptions = Options.Create(new CacheOptions 
        { 
            Enabled = true,
            AllShowsTtlMinutes = 60  // Custom 60 minute TTL
        });

        // Act
        var ttl = CacheOptions.ToTimeSpan(customOptions.Value.AllShowsTtlMinutes);

        // Assert
        Assert.Equal(TimeSpan.FromMinutes(60), ttl);
    }

    [Fact]
    public void CacheOptionsConfiguration_ShouldAllowDisabling()
    {
        // Arrange
        var disabledOptions = Options.Create(new CacheOptions { Enabled = false });

        // Act & Assert
        Assert.False(disabledOptions.Value.Enabled);
    }

    [Fact]
    public void AllShowsCacheKey_ShouldBeConstant()
    {
        // Arrange & Act
        var key1 = CacheKeys.ShowsActive;
        var key2 = CacheKeys.ShowsActive;

        // Assert
        Assert.Equal(key1, key2);
        Assert.Equal("shows:active", key1);
    }

    [Fact]
    public void UpcomingShowsCacheKey_ShouldBeDynamic()
    {
        // Arrange
        var template = CacheKeys.ShowsUpcoming;

        // Act
        var key1 = string.Format(template, 30);
        var key2 = string.Format(template, 60);

        // Assert
        Assert.NotEqual(key1, key2);
        Assert.Equal("shows:upcoming:30", key1);
        Assert.Equal("shows:upcoming:60", key2);
    }

    [Fact]
    public void TrendingShowsCacheKey_ShouldIncludeCount()
    {
        // Arrange & Act
        var key = string.Format(CacheKeys.ShowsTrending, 10);

        // Assert
        Assert.Equal("shows:trending:10", key);
    }

    [Fact]
    public void ShowDetailCacheKey_ShouldIncludeShowId()
    {
        // Arrange
        var showId = Guid.NewGuid();

        // Act
        var key = string.Format(CacheKeys.ShowDetail, showId);

        // Assert
        Assert.Contains(showId.ToString(), key);
        Assert.StartsWith("shows:detail:", key);
    }

    [Fact]
    public void ReviewsCacheKey_ShouldIncludeShowId()
    {
        // Arrange
        var showId = Guid.NewGuid();

        // Act
        var key = string.Format(CacheKeys.ReviewsForShow, showId);

        // Assert
        Assert.Contains(showId.ToString(), key);
        Assert.StartsWith("reviews:show:", key);
    }

    [Fact]
    public void CachePatternKeys_ShouldSupportWildcardMatching()
    {
        // Arrange & Act
        var showsPattern = CacheKeys.ShowsPattern;
        var reviewsPattern = CacheKeys.ReviewsPattern;

        // Assert
        Assert.Equal("shows:*", showsPattern);
        Assert.Equal("reviews:*", reviewsPattern);
        Assert.EndsWith("*", showsPattern);
        Assert.EndsWith("*", reviewsPattern);
    }

    [Fact]
    public void CacheOptionsTTLConfiguration_ShouldHaveAllProperties()
    {
        // Arrange
        var options = new CacheOptions();

        // Act & Assert - Verify all TTL properties exist and have default values
        Assert.True(options.TheatresTtlMinutes > 0);
        Assert.True(options.AllShowsTtlMinutes > 0);
        Assert.True(options.UpcomingShowsTtlMinutes > 0);
        Assert.True(options.TrendingShowsTtlMinutes > 0);
        Assert.True(options.ShowDetailTtlMinutes > 0);
        Assert.True(options.SearchResultsTtlMinutes > 0);
        Assert.True(options.FilteredShowsTtlMinutes > 0);
        Assert.True(options.ReviewsTtlMinutes > 0);
        Assert.True(options.UserFavoritesTtlMinutes > 0);
    }

    [Fact]
    public void CacheOptionsTTLValues_ShouldFollowReasonableDefaults()
    {
        // Arrange
        var options = new CacheOptions();

        // Act & Assert - TTLs should be reasonable (minutes)
        Assert.InRange(options.AllShowsTtlMinutes, 1, 1440);      // Between 1 minute and 24 hours
        Assert.InRange(options.TrendingShowsTtlMinutes, 1, 1440);
        Assert.InRange(options.ShowDetailTtlMinutes, 1, 1440);
        Assert.InRange(options.TheatresTtlMinutes, 60, 1440);    // Theatres cached longer
    }
}

