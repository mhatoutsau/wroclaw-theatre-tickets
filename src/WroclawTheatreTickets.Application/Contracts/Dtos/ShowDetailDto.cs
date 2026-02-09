namespace WroclawTheatreTickets.Application.Contracts.Dtos;

public class ShowDetailDto : ShowDto
{
    public string? FullDescription { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
}
