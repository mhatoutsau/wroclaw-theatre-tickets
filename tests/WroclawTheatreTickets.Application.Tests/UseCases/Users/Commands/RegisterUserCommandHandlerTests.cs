namespace WroclawTheatreTickets.Application.Tests.UseCases.Users.Commands;

using AutoMapper;
using FakeItEasy;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.UseCases.Users.Commands;
using WroclawTheatreTickets.Domain.Entities;
using Xunit;

public class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;
    private readonly IMapper _mapper;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository = A.Fake<IUserRepository>();
        _authService = A.Fake<IAuthenticationService>();
        _mapper = A.Fake<IMapper>();
        _handler = new RegisterUserCommandHandler(_userRepository, _authService, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldRegisterUser()
    {
        // Arrange
        var request = new UserRegistrationRequest
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };
        var command = new RegisterUserCommand(request);
        var hashedPassword = "hashed_password";
        var accessToken = "test_token";

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns((User?)null);
        A.CallTo(() => _authService.HashPasswordAsync(request.Password))
            .Returns(hashedPassword);
        A.CallTo(() => _authService.GenerateJwtTokenAsync(A<Guid>._, request.Email, "User"))
            .Returns(accessToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(accessToken, result.AccessToken);
        A.CallTo(() => _userRepository.AddAsync(A<User>.That.Matches(u =>
            u.Email == request.Email &&
            u.FirstName == request.FirstName &&
            u.LastName == request.LastName &&
            u.PasswordHash == hashedPassword
        ))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var existingUser = User.Create("existing@example.com");
        var request = new UserRegistrationRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "Jane",
            LastName = "Doe"
        };
        var command = new RegisterUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(request.Email))
            .Returns(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

        // Verify that AddAsync was never called
        A.CallTo(() => _userRepository.AddAsync(A<User>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_ShouldHashPassword()
    {
        // Arrange
        var request = new UserRegistrationRequest
        {
            Email = "test@example.com",
            Password = "PlainPassword123!",
            FirstName = "Test",
            LastName = "User"
        };
        var command = new RegisterUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(A<string>._))
            .Returns((User?)null);
        A.CallTo(() => _authService.HashPasswordAsync(A<string>._))
            .Returns("hashed_password");
        A.CallTo(() => _authService.GenerateJwtTokenAsync(A<Guid>._, A<string>._, A<string>._))
            .Returns("token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _authService.HashPasswordAsync(request.Password))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_ShouldGenerateJwtToken()
    {
        // Arrange
        var request = new UserRegistrationRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };
        var command = new RegisterUserCommand(request);

        A.CallTo(() => _userRepository.GetByEmailAsync(A<string>._))
            .Returns((User?)null);
        A.CallTo(() => _authService.HashPasswordAsync(A<string>._))
            .Returns("hashed");
        A.CallTo(() => _authService.GenerateJwtTokenAsync(A<Guid>._, A<string>._, A<string>._))
            .Returns("jwt_token");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _authService.GenerateJwtTokenAsync(A<Guid>._, request.Email, "User"))
            .MustHaveHappenedOnceExactly();
    }
}
