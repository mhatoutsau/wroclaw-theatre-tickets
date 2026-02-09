namespace WroclawTheatreTickets.Domain.Entities;

using WroclawTheatreTickets.Domain.Common;

public class Theatre : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = "Wrocław";
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? BookingUrl { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Show> Shows { get; set; } = new List<Show>();

    public static Theatre Create(
        string name,
        string address,
        string? phoneNumber = null,
        string? email = null,
        string? websiteUrl = null,
        string? bookingUrl = null)
    {
        return new Theatre
        {
            Id = Guid.NewGuid(),
            Name = name,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            WebsiteUrl = websiteUrl,
            BookingUrl = bookingUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}
