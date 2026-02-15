namespace WroclawTheatreTickets.Domain.Tests.Entities;

using WroclawTheatreTickets.Domain.Entities;
using Xunit;
using AutoFixture;

public class TheatreTests
{
    private readonly Fixture _fixture;

    public TheatreTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Create_ShouldReturnValidTheatre()
    {
        // Arrange
        var name = "Opera Wrocławska";
        var address = "ul. Świdnicka 35";
        var phoneNumber = "+48 71 370 8812";
        var email = "opera@opera.wroclaw.pl";
        var websiteUrl = "https://opera.wroclaw.pl";
        var bookingUrl = "https://opera.wroclaw.pl/tickets";

        // Act
        var theatre = Theatre.Create(name, address, phoneNumber, email, websiteUrl, bookingUrl);

        // Assert
        Assert.NotNull(theatre);
        Assert.NotEqual(Guid.Empty, theatre.Id);
        Assert.Equal(name, theatre.Name);
        Assert.Equal(address, theatre.Address);
        Assert.Equal(phoneNumber, theatre.PhoneNumber);
        Assert.Equal(email, theatre.Email);
        Assert.Equal(websiteUrl, theatre.WebsiteUrl);
        Assert.Equal(bookingUrl, theatre.BookingUrl);
        Assert.True(theatre.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithMinimalData_ShouldReturnValidTheatre()
    {
        // Arrange
        var name = "Test Theatre";
        var address = "Test Address";

        // Act
        var theatre = Theatre.Create(name, address);

        // Assert
        Assert.NotNull(theatre);
        Assert.Equal(name, theatre.Name);
        Assert.Equal(address, theatre.Address);
        Assert.Null(theatre.PhoneNumber);
        Assert.Null(theatre.Email);
        Assert.Null(theatre.WebsiteUrl);
        Assert.Null(theatre.BookingUrl);
    }

    [Fact]
    public void Theatre_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var theatre = new Theatre();

        // Assert
        Assert.True(theatre.IsActive);
        Assert.Equal("Wrocław", theatre.City);
        Assert.Empty(theatre.Shows);
    }
}
