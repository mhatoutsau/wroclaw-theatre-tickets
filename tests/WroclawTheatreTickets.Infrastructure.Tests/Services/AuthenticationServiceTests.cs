namespace WroclawTheatreTickets.Infrastructure.Tests.Services;

using Microsoft.Extensions.Configuration;
using WroclawTheatreTickets.Infrastructure.Services;
using Xunit;
using FakeItEasy;
using System.IdentityModel.Tokens.Jwt;

public class AuthenticationServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        // Create fake configuration with JWT settings
        _configuration = A.Fake<IConfiguration>();
        A.CallTo(() => _configuration["Jwt:Secret"]).Returns("ThisIsAVeryLongSecretKeyForTesting12345678901234567890");
        A.CallTo(() => _configuration["Jwt:Issuer"]).Returns("TestIssuer");
        A.CallTo(() => _configuration["Jwt:Audience"]).Returns("TestAudience");

        _authenticationService = new AuthenticationService(_configuration);
    }

    [Fact]
    public async Task HashPasswordAsync_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash = await _authenticationService.HashPasswordAsync(password);

        // Assert
        Assert.NotNull(hash);
        Assert.NotEqual(password, hash);
        Assert.True(hash.StartsWith("$2"));  // BCrypt hashes start with $2
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = await _authenticationService.HashPasswordAsync(password);

        // Act
        var result = await _authenticationService.VerifyPasswordAsync(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task VerifyPasswordAsync_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = await _authenticationService.HashPasswordAsync(password);

        // Act
        var result = await _authenticationService.VerifyPasswordAsync(wrongPassword, hash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var role = "User";

        // Act
        var token = await _authenticationService.GenerateJwtTokenAsync(userId, email, role);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Contains("TestAudience", jwtToken.Audiences);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" && c.Value == email);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && c.Value == userId.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == role);
    }

    [Fact]
    public async Task GenerateTokensAsync_ShouldReturnAccessAndRefreshTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var role = "Admin";

        // Act
        var (accessToken, refreshToken) = await _authenticationService.GenerateTokensAsync(userId, email, role);

        // Assert
        Assert.NotNull(accessToken);
        Assert.NotEmpty(accessToken);
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
        Assert.NotEqual(accessToken, refreshToken);
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("Moderator")]
    public async Task GenerateJwtTokenAsync_WithDifferentRoles_ShouldIncludeRoleClaim(string role)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = await _authenticationService.GenerateJwtTokenAsync(userId, email, role);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == role);
    }

    [Fact]
    public async Task GenerateTokensAsync_MultipleCalls_ShouldGenerateUniqueRefreshTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var role = "User";

        // Act
        var (_, refreshToken1) = await _authenticationService.GenerateTokensAsync(userId, email, role);
        var (_, refreshToken2) = await _authenticationService.GenerateTokensAsync(userId, email, role);

        // Assert
        Assert.NotEqual(refreshToken1, refreshToken2);
    }
}
