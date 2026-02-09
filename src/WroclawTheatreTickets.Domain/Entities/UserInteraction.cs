namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

public class UserFavorite : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ShowId { get; set; }
    public Show? Show { get; set; }

    public static UserFavorite Create(Guid userId, Guid showId)
    {
        return new UserFavorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShowId = showId,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class ViewHistory : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ShowId { get; set; }
    public Show? Show { get; set; }

    public DateTime ViewedAt { get; set; }

    public static ViewHistory Create(Guid userId, Guid showId)
    {
        return new ViewHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShowId = showId,
            ViewedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class Review : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ShowId { get; set; }
    public Show? Show { get; set; }

    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = false;

    public static Review Create(Guid userId, Guid showId, int rating, string? comment = null)
    {
        return new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShowId = showId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class Notification : Entity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid? ShowId { get; set; }
    public Show? Show { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;

    public static Notification Create(Guid userId, string title, NotificationType type, string? message = null, Guid? showId = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ShowId = showId,
            Title = title,
            Message = message,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public enum NotificationType
{
    EventReminder,          // Reminder about upcoming event
    NewEventInCategory,     // New event in favorite category
    ReviewResponse,         // Admin response to review
    SystemAlert,            // System-wide announcements
    WeeklyDigest            // Weekly newsletter
}
