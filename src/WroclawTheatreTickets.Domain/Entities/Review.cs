namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

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
