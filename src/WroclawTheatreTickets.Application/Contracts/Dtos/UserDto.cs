namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsEmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
