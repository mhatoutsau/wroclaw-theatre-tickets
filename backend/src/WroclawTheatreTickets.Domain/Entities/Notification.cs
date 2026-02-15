namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

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
