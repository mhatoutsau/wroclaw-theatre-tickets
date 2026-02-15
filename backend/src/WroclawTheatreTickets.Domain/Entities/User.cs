namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

public class User : Entity
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PasswordHash { get; set; }
    public string? ExternalId { get; set; } // For OAuth providers
    public string? Provider { get; set; } // e.g., "Google", "Facebook", "Local"

    public bool IsEmailVerified { get; set; } = false;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Notification preferences
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnablePushNotifications { get; set; } = true;
    public string? PreferredCategories { get; set; } // JSON array of PerformanceType

    // Role
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    public ICollection<ViewHistory> ViewHistory { get; set; } = new List<ViewHistory>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public static User Create(string email, string? firstName = null, string? lastName = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static User CreateOAuth(string email, string externalId, string provider, string? firstName = null, string? lastName = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            ExternalId = externalId,
            Provider = provider,
            IsEmailVerified = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum UserRole
{
    User,
    Moderator,
    Admin
}
