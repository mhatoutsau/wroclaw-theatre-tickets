namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class AuthenticationResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
