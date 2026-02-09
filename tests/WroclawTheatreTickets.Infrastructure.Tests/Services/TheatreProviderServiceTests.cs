namespace WroclawTheatreTickets.Infrastructure.Tests.Services;

using FakeItEasy;
using Microsoft.Extensions.Logging;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Services;
using Xunit;

public class TheatreProviderServiceTests
{
    private readonly ITheatreRepository _theatreRepository;
    private readonly ILogger<TheatreProviderService> _logger;
    private readonly TheatreProviderService _service;

    public TheatreProviderServiceTests()
    {
        _theatreRepository = A.Fake<ITheatreRepository>();
        _logger = A.Fake<ILogger<TheatreProviderService>>();
        _service = new TheatreProviderService(_theatreRepository, _logger);
    }

    [Fact]
    public async Task GetOrCreateTheatreAsync_WithExistingTheatre_ReturnsExisting()
    {
        // Arrange
        var theatreName = "Teatr Polski we Wrocławiu";
        var existingTheatre = Theatre.Create(theatreName, "Wrocław, Poland");

        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName))
            .Returns(existingTheatre);

        // Act
        var result = await _service.GetOrCreateTheatreAsync(theatreName, "Wrocław", "Wrocław, Poland");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingTheatre.Id, result.Id);
        Assert.Equal(theatreName, result.Name);
        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrCreateTheatreAsync_WithNonExistentTheatre_CreatesAndReturnsNew()
    {
        // Arrange
        var theatreName = "Teatr Polski we Wrocławiu";
        var city = "Wrocław";
        var address = "Wrocław, Poland";

        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName))
            .Returns((Theatre?)null);

        A.CallTo(() => _theatreRepository.AddAsync(A<Theatre>._))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GetOrCreateTheatreAsync(theatreName, city, address);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(theatreName, result.Name);
        Assert.Equal(city, result.City);
        Assert.Equal(address, result.Address);
        Assert.True(result.IsActive);
        
        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _theatreRepository.AddAsync(A<Theatre>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrCreateTheatreAsync_WithRepositoryGetError_ReturnsNull()
    {
        // Arrange
        var theatreName = "Teatr Polski we Wrocławiu";
        var exception = new Exception("Database error");

        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName))
            .Throws(exception);

        // Act
        var result = await _service.GetOrCreateTheatreAsync(theatreName, "Wrocław", "Wrocław, Poland");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetOrCreateTheatreAsync_WithRepositoryAddError_ReturnsNull()
    {
        // Arrange
        var theatreName = "Teatr Polski we Wrocławiu";
        var exception = new Exception("Database error on add");

        A.CallTo(() => _theatreRepository.GetByNameAsync(theatreName))
            .Returns((Theatre?)null);

        A.CallTo(() => _theatreRepository.AddAsync(A<Theatre>._))
            .Throws(exception);

        // Act
        var result = await _service.GetOrCreateTheatreAsync(theatreName, "Wrocław", "Wrocław, Poland");

        // Assert
        Assert.Null(result);
    }
}
