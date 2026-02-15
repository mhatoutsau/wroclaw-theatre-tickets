namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Password { get; set; } = string.Empty;
}
