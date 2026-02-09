namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ShowId { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    public Guid ShowId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
}

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public Guid? ShowId { get; set; }
    public DateTime CreatedAt { get; set; }
}
