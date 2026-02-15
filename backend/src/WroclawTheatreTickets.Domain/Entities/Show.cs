namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

public enum PerformanceType
{
    Ballet,
    Opera,
    Play,
    Comedy,
    Drama,
    Musical,
    Concert,
    Other
}

public enum AgeRestriction
{
    All,           // 0+
    Age6,          // 6+
    Age12,         // 12+
    Age16,         // 16+
    Age18          // 18+
}

public class Show : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FullDescription { get; set; }
    public PerformanceType Type { get; set; }
    public Guid TheatreId { get; set; }
    public Theatre? Theatre { get; set; }

    // Cast and crew
    public string? Director { get; set; }
    public string? Cast { get; set; }

    // Show details
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Language { get; set; }

    // Pricing and restrictions
    public decimal? MinimumPrice { get; set; }
    public decimal? MaximumPrice { get; set; }
    public AgeRestriction AgeRestriction { get; set; }

    // Images and links
    public string? PosterUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? TicketUrl { get; set; }

    // Metadata
    public bool IsActive { get; set; } = true;
    public int ViewCount { get; set; } = 0;
    public double Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public string? ExternalId { get; set; }

    public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public static Show Create(
        string title,
        Guid theatreId,
        PerformanceType type,
        DateTime startDateTime)
    {
        return new Show
        {
            Id = Guid.NewGuid(),
            Title = title,
            TheatreId = theatreId,
            Type = type,
            StartDateTime = startDateTime,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void IncrementViewCount()
    {
        ViewCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(double newRating, int reviewCount)
    {
        Rating = newRating;
        ReviewCount = reviewCount;
        UpdatedAt = DateTime.UtcNow;
    }
}
