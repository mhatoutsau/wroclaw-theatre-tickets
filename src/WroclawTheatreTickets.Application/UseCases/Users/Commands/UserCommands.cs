namespace WroclawTheatreTickets.Application.UseCases.Users.Commands;

using MediatR;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Domain.Entities;
using AutoMapper;

public record RegisterUserCommand(UserRegistrationRequest Request) : IRequest<AuthenticationResponse>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(IUserRepository userRepository, IAuthenticationService authService, IMapper mapper)
    {
        _userRepository = userRepository;
        _authService = authService;
        _mapper = mapper;
    }

    public async Task<AuthenticationResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Request.Email);
        if (existing != null)
            throw new Exception($"User with email {request.Request.Email} already exists");

        var passwordHash = await _authService.HashPasswordAsync(request.Request.Password);
        var user = User.Create(request.Request.Email, request.Request.FirstName, request.Request.LastName);
        user.PasswordHash = passwordHash;

        await _userRepository.AddAsync(user);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Id, user.Email, "User");
        return new AuthenticationResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}

public record LoginUserCommand(UserLoginRequest Request) : IRequest<AuthenticationResponse>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;

    public LoginUserCommandHandler(IUserRepository userRepository, IAuthenticationService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthenticationResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Request.Email);
        if (user == null || user.PasswordHash == null)
            throw new Exception("Invalid email or password");

        var isValid = await _authService.VerifyPasswordAsync(request.Request.Password, user.PasswordHash);
        if (!isValid)
            throw new Exception("Invalid email or password");

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Id, user.Email, user.Role.ToString());
        return new AuthenticationResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}

public record OAuthLoginCommand(OAuthRequest Request) : IRequest<AuthenticationResponse>;

public class OAuthLoginCommandHandler : IRequestHandler<OAuthLoginCommand, AuthenticationResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationService _authService;

    public OAuthLoginCommandHandler(IUserRepository userRepository, IAuthenticationService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<AuthenticationResponse> Handle(OAuthLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByExternalIdAsync(request.Request.ExternalId, request.Request.Provider);

        if (user == null)
        {
            user = User.CreateOAuth(request.Request.Email, request.Request.ExternalId, request.Request.Provider,
                request.Request.FirstName, request.Request.LastName);
            await _userRepository.AddAsync(user);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var accessToken = await _authService.GenerateJwtTokenAsync(user.Id, user.Email, user.Role.ToString());
        return new AuthenticationResponse
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };
    }
}
