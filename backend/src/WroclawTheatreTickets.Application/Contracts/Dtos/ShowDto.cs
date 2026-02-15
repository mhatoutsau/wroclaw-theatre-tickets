namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class ShowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public TheatreDto? Theatre { get; set; }
    public string? Director { get; set; }
    public string? Cast { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string? Language { get; set; }
    public decimal? MinimumPrice { get; set; }
    public decimal? MaximumPrice { get; set; }
    public string AgeRestriction { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? TicketUrl { get; set; }
    public int ViewCount { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsFavorite { get; set; }
}
