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

public class UserProfileDto : UserDto
{
    public bool EnableEmailNotifications { get; set; }
    public bool EnablePushNotifications { get; set; }
    public ICollection<string>? PreferredCategories { get; set; }
}

public class UserRegistrationRequest
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Password { get; set; } = string.Empty;
}

public class UserLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class OAuthRequest
{
    public string Email { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty; // "google", "facebook"
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class AuthenticationResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}
