namespace WroclawTheatreTickets.Application.Tests.UseCases.Users.Commands;

using FakeItEasy;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.UseCases.Users.Commands;
using WroclawTheatreTickets.Domain.Entities;
using Xunit;

public class LoginUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _authService = A.Fake<IAuthenticationService>();
        _handler = new LoginUserCommandHandler(_userRepository, _authService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var user = User.Create("user@example.com", "John", "Doe");
        user.PasswordHash = "hashed_password";

        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "Password123!"
        };
        var command = new LoginUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns(user);
        A.CallTo(() => _authService.VerifyPasswordAsync(request.Password, user.PasswordHash))
            .Returns(true);
        A.CallTo(() => _authService.GenerateJwtTokenAsync(user.Id, user.Email, user.Role.ToString()))
            .Returns("jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("jwt_token", result.AccessToken);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var request = new UserLoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };
        var command = new LoginUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        Assert.Contains("Invalid email or password", exception.Message);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("user@example.com");
        user.PasswordHash = "hashed_password";

        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "WrongPassword123!"
        };
        var command = new LoginUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns(user);
        A.CallTo(() => _authService.VerifyPasswordAsync(request.Password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        Assert.Contains("Invalid email or password", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLastLoginAt()
    {
        // Arrange
        var user = User.Create("user@example.com");
        user.PasswordHash = "hashed_password";

        var request = new UserLoginRequest
        {
            Email = "user@example.com",
            Password = "Password123!"
        };
        var command = new LoginUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns(user);
        A.CallTo(() => _authService.VerifyPasswordAsync(A<string>._, A<string>._))
            .Returns(true);
        A.CallTo(() => _authService.GenerateJwtTokenAsync(A<Guid>._, A<string>._, A<string>._))
            .Returns("token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(user.LastLoginAt);
        Assert.True(user.LastLoginAt <= DateTime.UtcNow);
        A.CallTo(() => _userRepository.UpdateAsync(user)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WithUserWithoutPasswordHash_ShouldThrowException()
    {
        // Arrange
        var user = User.Create("oauthuser@example.com");
        user.PasswordHash = null; // OAuth user without password

        var request = new UserLoginRequest
        {
            Email = "oauthuser@example.com",
            Password = "Password123!"
        };
        var command = new LoginUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        Assert.Contains("Invalid email or password", exception.Message);
    }
}
