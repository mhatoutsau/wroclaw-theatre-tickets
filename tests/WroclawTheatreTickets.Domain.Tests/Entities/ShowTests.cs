namespace WroclawTheatreTickets.Domain.Tests.Entities;

using WroclawTheatreTickets.Domain.Entities;
using Xunit;
using AutoFixture;

public class ShowTests
{
    private readonly Fixture _fixture;

    public ShowTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_ShouldReturnValidShow()
    {
        // Arrange
        var title = _fixture.Create<string>();
        var theatreId = Guid.NewGuid();
        var type = PerformanceType.Play;
        var startDateTime = DateTime.UtcNow.AddDays(7);

        // Act
        var show = Show.Create(title, theatreId, type, startDateTime);

        // Assert
        Assert.NotNull(show);
        Assert.NotEqual(Guid.Empty, show.Id);
        Assert.Equal(title, show.Title);
        Assert.Equal(theatreId, show.TheatreId);
        Assert.Equal(type, show.Type);
        Assert.Equal(startDateTime, show.StartDateTime);
        Assert.True(show.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void IncrementViewCount_ShouldIncreaseViewCountByOne()
    {
        // Arrange
        var show = Show.Create("Test Show", Guid.NewGuid(), PerformanceType.Ballet, DateTime.UtcNow.AddDays(1));
        var initialViewCount = show.ViewCount;

        // Act
        show.IncrementViewCount();

        // Assert
        Assert.Equal(initialViewCount + 1, show.ViewCount);
        Assert.True(show.UpdatedAt.HasValue);
    }

    [Fact]
    public void IncrementViewCount_ShouldUpdateTimestamp()
    {
        // Arrange
        var show = Show.Create("Test Show", Guid.NewGuid(), PerformanceType.Opera, DateTime.UtcNow.AddDays(1));
        var initialUpdatedAt = show.UpdatedAt;

        // Act
        show.IncrementViewCount();

        // Assert
        Assert.NotEqual(initialUpdatedAt, show.UpdatedAt);
        Assert.True(show.UpdatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(4.5, 10)]
    [InlineData(3.0, 5)]
    [InlineData(5.0, 20)]
    public void UpdateRating_ShouldUpdateRatingAndReviewCount(double newRating, int reviewCount)
    {
        // Arrange
        var show = Show.Create("Test Show", Guid.NewGuid(), PerformanceType.Musical, DateTime.UtcNow.AddDays(1));

        // Act
        show.UpdateRating(newRating, reviewCount);

        // Assert
        Assert.Equal(newRating, show.Rating);
        Assert.Equal(reviewCount, show.ReviewCount);
        Assert.True(show.UpdatedAt.HasValue);
    }

    [Fact]
    public void Show_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var show = new Show();

        // Assert
        Assert.True(show.IsActive);
        Assert.Equal(0, show.ViewCount);
        Assert.Equal(0, show.Rating);
        Assert.Equal(0, show.ReviewCount);
        Assert.Empty(show.UserFavorites);
        Assert.Empty(show.Reviews);
    }
}
