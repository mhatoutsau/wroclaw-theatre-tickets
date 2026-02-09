namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class OAuthRequest
{
    public string Email { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty; // "google", "facebook"
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
