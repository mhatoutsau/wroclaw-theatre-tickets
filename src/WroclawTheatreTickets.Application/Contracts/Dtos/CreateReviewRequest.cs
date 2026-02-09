namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class CreateReviewRequest
{
    public Guid ShowId { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
}
