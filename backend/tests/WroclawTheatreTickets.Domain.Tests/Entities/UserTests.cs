namespace WroclawTheatreTickets.Domain.Tests.Entities;

using WroclawTheatreTickets.Domain.Entities;
using Xunit;
using AutoFixture;

public class UserTests
{
    private readonly Fixture _fixture;

    public UserTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_ShouldReturnValidUser()
    {
        // Arrange
        var email = _fixture.Create<string>() + "@test.com";
        var firstName = _fixture.Create<string>();
        var lastName = _fixture.Create<string>();

        // Act
        var user = User.Create(email, firstName, lastName);

        // Assert
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithoutNames_ShouldReturnValidUser()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var user = User.Create(email);

        // Assert
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Null(user.FirstName);
        Assert.Null(user.LastName);
    }

    [Fact]
    public void CreateOAuth_ShouldReturnValidOAuthUser()
    {
        // Arrange
        var email = "oauth@example.com";
        var externalId = "google-123456";
        var provider = "Google";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = User.CreateOAuth(email, externalId, provider, firstName, lastName);

        // Assert
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(externalId, user.ExternalId);
        Assert.Equal(provider, user.Provider);
        Assert.Equal(firstName, user.FirstName);
        Assert.Equal(lastName, user.LastName);
        Assert.True(user.IsEmailVerified);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void User_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.False(user.IsEmailVerified);
        Assert.True(user.IsActive);
        Assert.True(user.EnableEmailNotifications);
        Assert.True(user.EnablePushNotifications);
        Assert.Equal(UserRole.User, user.Role);
        Assert.Empty(user.Favorites);
        Assert.Empty(user.ViewHistory);
        Assert.Empty(user.Reviews);
        Assert.Empty(user.Notifications);
    }

    [Theory]
    [InlineData("Google")]
    [InlineData("Facebook")]
    [InlineData("Local")]
    public void CreateOAuth_WithDifferentProviders_ShouldSetProviderCorrectly(string provider)
    {
        // Arrange
        var email = "user@example.com";
        var externalId = $"{provider.ToLower()}-12345";

        // Act
        var user = User.CreateOAuth(email, externalId, provider);

        // Assert
        Assert.Equal(provider, user.Provider);
        Assert.Equal(externalId, user.ExternalId);
        Assert.True(user.IsEmailVerified);
    }
}
