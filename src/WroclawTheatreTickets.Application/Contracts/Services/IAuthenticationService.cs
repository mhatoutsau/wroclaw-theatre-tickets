namespace WroclawTheatreTickets.Application.Contracts.Services;

public interface IAuthenticationService
{
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hash);
    Task<string> GenerateJwtTokenAsync(Guid userId, string email, string role);
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(Guid userId, string email, string role);
}
