namespace WroclawTheatreTickets.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WroclawTheatreTickets.Application.Contracts.Services;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

public class AuthenticationService : IAuthenticationService
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public AuthenticationService(IConfiguration configuration)
    {
        _jwtSecret = configuration["Jwt:Secret"] ?? throw new Exception("JWT Secret not configured");
        _jwtIssuer = configuration["Jwt:Issuer"] ?? "WroclawTheatreTickets";
        _jwtAudience = configuration["Jwt:Audience"] ?? "WroclawTheatreTicketsUsers";
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.FromResult(BCrypt.HashPassword(password));
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        return await Task.FromResult(BCrypt.Verify(password, hash));
    }

    public async Task<string> GenerateJwtTokenAsync(Guid userId, string email, string role)
    {
        var (token, _) = await GenerateTokensAsync(userId, email, role);
        return token;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(Guid userId, string email, string role)
    {
        return await Task.FromResult(GenerateTokens(userId, email, role));
    }

    private (string AccessToken, string RefreshToken) GenerateTokens(Guid userId, string email, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role),
            new("sub", userId.ToString())
        };

        var accessToken = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

        // Simple refresh token (in production, store in database)
        var refreshToken = BCrypt.HashPassword(Guid.NewGuid().ToString());

        return (accessTokenString, refreshToken);
    }
}
