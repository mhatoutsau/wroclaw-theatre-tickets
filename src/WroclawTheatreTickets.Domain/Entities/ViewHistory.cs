namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

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
