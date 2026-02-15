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
