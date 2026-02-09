namespace WroclawTheatreTickets.Domain.Tests.Entities;

using WroclawTheatreTickets.Domain.Entities;
using Xunit;

public class UserInteractionTests
{
    [Fact]
    public void UserFavorite_Create_ShouldReturnValidFavorite()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var showId = Guid.NewGuid();

        // Act
        var favorite = UserFavorite.Create(userId, showId);

        // Assert
        Assert.NotNull(favorite);
        Assert.NotEqual(Guid.Empty, favorite.Id);
        Assert.Equal(userId, favorite.UserId);
        Assert.Equal(showId, favorite.ShowId);
        Assert.True(favorite.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void ViewHistory_Create_ShouldReturnValidViewHistory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var showId = Guid.NewGuid();

        // Act
        var viewHistory = ViewHistory.Create(userId, showId);

        // Assert
        Assert.NotNull(viewHistory);
        Assert.NotEqual(Guid.Empty, viewHistory.Id);
        Assert.Equal(userId, viewHistory.UserId);
        Assert.Equal(showId, viewHistory.ShowId);
        Assert.True(viewHistory.ViewedAt <= DateTime.UtcNow);
        Assert.True(viewHistory.CreatedAt <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(1, "Poor")]
    [InlineData(3, "Average")]
    [InlineData(5, "Excellent")]
    public void Review_Create_ShouldReturnValidReview(int rating, string comment)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var showId = Guid.NewGuid();

        // Act
        var review = Review.Create(userId, showId, rating, comment);

        // Assert
        Assert.NotNull(review);
        Assert.NotEqual(Guid.Empty, review.Id);
        Assert.Equal(userId, review.UserId);
        Assert.Equal(showId, review.ShowId);
        Assert.Equal(rating, review.Rating);
        Assert.Equal(comment, review.Comment);
        Assert.False(review.IsApproved);
        Assert.True(review.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Review_Create_WithoutComment_ShouldReturnValidReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var showId = Guid.NewGuid();
        var rating = 4;

        // Act
        var review = Review.Create(userId, showId, rating);

        // Assert
        Assert.NotNull(review);
        Assert.Equal(rating, review.Rating);
        Assert.Null(review.Comment);
    }

    [Theory]
    [InlineData(NotificationType.EventReminder)]
    [InlineData(NotificationType.NewEventInCategory)]
    [InlineData(NotificationType.ReviewResponse)]
    [InlineData(NotificationType.SystemAlert)]
    [InlineData(NotificationType.WeeklyDigest)]
    public void Notification_Create_ShouldReturnValidNotification(NotificationType type)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "Test message";
        var showId = Guid.NewGuid();

        // Act
        var notification = Notification.Create(userId, title, type, message, showId);

        // Assert
        Assert.NotNull(notification);
        Assert.NotEqual(Guid.Empty, notification.Id);
        Assert.Equal(userId, notification.UserId);
        Assert.Equal(title, notification.Title);
        Assert.Equal(message, notification.Message);
        Assert.Equal(type, notification.Type);
        Assert.Equal(showId, notification.ShowId);
        Assert.False(notification.IsRead);
        Assert.True(notification.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Notification_Create_WithoutOptionalFields_ShouldReturnValidNotification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var title = "Simple Notification";
        var type = NotificationType.SystemAlert;

        // Act
        var notification = Notification.Create(userId, title, type);

        // Assert
        Assert.NotNull(notification);
        Assert.Equal(title, notification.Title);
        Assert.Null(notification.Message);
        Assert.Null(notification.ShowId);
    }
}
