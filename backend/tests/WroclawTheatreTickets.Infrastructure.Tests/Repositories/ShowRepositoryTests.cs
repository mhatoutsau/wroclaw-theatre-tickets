namespace WroclawTheatreTickets.Infrastructure.Tests.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;
using WroclawTheatreTickets.Infrastructure.Repositories;
using Xunit;

public class ShowRepositoryTests : IDisposable
{
    private readonly TheatreDbContext _context;
    private readonly ShowRepository _repository;
    private readonly Theatre _testTheatre;

    public ShowRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TheatreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TheatreDbContext(options);
        _repository = new ShowRepository(_context);

        // Create a test theatre for shows
        _testTheatre = Theatre.Create("Test Theatre", "Test Address");
        _context.Theatres.Add(_testTheatre);
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_ShouldAddShowToDatabase()
    {
        // Arrange
        var show = Show.Create("Test Show", _testTheatre.Id, PerformanceType.Play, DateTime.UtcNow.AddDays(7));

        // Act
        await _repository.AddAsync(show);

        // Assert
        var savedShow = await _context.Shows.FindAsync(show.Id);
        Assert.NotNull(savedShow);
        Assert.Equal(show.Title, savedShow.Title);
        Assert.Equal(_testTheatre.Id, savedShow.TheatreId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnShow()
    {
        // Arrange
        var show = Show.Create("Hamlet", _testTheatre.Id, PerformanceType.Drama, DateTime.UtcNow.AddDays(5));
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(show.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(show.Id, result.Id);
        Assert.Equal(show.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldIncludeTheatreAndReviews()
    {
        // Arrange
        var show = Show.Create("Faust", _testTheatre.Id, PerformanceType.Opera, DateTime.UtcNow.AddDays(3));
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithDetailsAsync(show.Id);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Theatre);
        Assert.Equal(_testTheatre.Name, result.Theatre.Name);
    }

    [Fact]
    public async Task GetByExternalIdAsync_ShouldReturnShowWithMatchingExternalId()
    {
        // Arrange
        var show = Show.Create("External Show", _testTheatre.Id, PerformanceType.Concert, DateTime.UtcNow.AddDays(10));
        show.ExternalId = "ext-12345";
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByExternalIdAsync("ext-12345");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(show.Id, result.Id);
        Assert.Equal("ext-12345", result.ExternalId);
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveShows()
    {
        // Arrange
        var activeShow = Show.Create("Active Show", _testTheatre.Id, PerformanceType.Ballet, DateTime.UtcNow.AddDays(2));
        var inactiveShow = Show.Create("Inactive Show", _testTheatre.Id, PerformanceType.Musical, DateTime.UtcNow.AddDays(4));
        inactiveShow.IsActive = false;

        await _context.Shows.AddRangeAsync(activeShow, inactiveShow);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetActiveAsync();

        // Assert
        Assert.Single(results);
        Assert.Equal(activeShow.Id, results.First().Id);
    }

    [Fact]
    public async Task FilterAsync_WithTypeFilter_ShouldReturnMatchingShows()
    {
        // Arrange
        var playShow = Show.Create("Play Show", _testTheatre.Id, PerformanceType.Play, DateTime.UtcNow.AddDays(5));
        var operaShow = Show.Create("Opera Show", _testTheatre.Id, PerformanceType.Opera, DateTime.UtcNow.AddDays(6));
        await _context.Shows.AddRangeAsync(playShow, operaShow);
        await _context.SaveChangesAsync();

        var criteria = new ShowFilterCriteria { Type = "Play" };

        // Act
        var results = await _repository.FilterAsync(criteria);

        // Assert
        Assert.Single(results);
        Assert.Equal(playShow.Id, results.First().Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateShowProperties()
    {
        // Arrange
        var show = Show.Create("Original Title", _testTheatre.Id, PerformanceType.Comedy, DateTime.UtcNow.AddDays(7));
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        show.Title = "Updated Title";
        show.Description = "Updated Description";
        await _repository.UpdateAsync(show);

        // Assert
        var updatedShow = await _context.Shows.FindAsync(show.Id);
        Assert.NotNull(updatedShow);
        Assert.Equal("Updated Title", updatedShow.Title);
        Assert.Equal("Updated Description", updatedShow.Description);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveShowFromDatabase()
    {
        // Arrange
        var show = Show.Create("Show to Delete", _testTheatre.Id, PerformanceType.Other, DateTime.UtcNow.AddDays(15));
        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(show.Id);

        // Assert
        var deletedShow = await _context.Shows.FindAsync(show.Id);
        Assert.Null(deletedShow);
    }
}
