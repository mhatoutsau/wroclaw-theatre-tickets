namespace WroclawTheatreTickets.Application.Contracts.Repositories;

public class ShowFilterCriteria
{
    public string? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? TheatreId { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public string? Language { get; set; }
    public string? AgeRestriction { get; set; }
}
