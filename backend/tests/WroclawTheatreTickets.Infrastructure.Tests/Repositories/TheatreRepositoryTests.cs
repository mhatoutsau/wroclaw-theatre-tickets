namespace WroclawTheatreTickets.Infrastructure.Tests.Repositories;

using Microsoft.EntityFrameworkCore;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Data;
using WroclawTheatreTickets.Infrastructure.Repositories;
using Xunit;

public class TheatreRepositoryTests : IDisposable
{
    private readonly TheatreDbContext _context;
    private readonly TheatreRepository _repository;

    public TheatreRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TheatreDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TheatreDbContext(options);
        _repository = new TheatreRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTheatreToDatabase()
    {
        // Arrange
        var theatre = Theatre.Create("Test Theatre", "Test Address", "+48123456789", "test@example.com");

        // Act
        await _repository.AddAsync(theatre);

        // Assert
        var savedTheatre = await _context.Theatres.FindAsync(theatre.Id);
        Assert.NotNull(savedTheatre);
        Assert.Equal(theatre.Name, savedTheatre.Name);
        Assert.Equal(theatre.Address, savedTheatre.Address);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTheatre()
    {
        // Arrange
        var theatre = Theatre.Create("Opera", "Świdnicka 35");
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(theatre.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(theatre.Id, result.Id);
        Assert.Equal(theatre.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTheatres()
    {
        // Arrange
        var theatre1 = Theatre.Create("Theatre 1", "Address 1");
        var theatre2 = Theatre.Create("Theatre 2", "Address 2");
        await _context.Theatres.AddRangeAsync(theatre1, theatre2);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task GetActiveAsync_ShouldReturnOnlyActiveTheatres()
    {
        // Arrange
        var activeTheatre = Theatre.Create("Active Theatre", "Address 1");
        var inactiveTheatre = Theatre.Create("Inactive Theatre", "Address 2");
        inactiveTheatre.IsActive = false;

        await _context.Theatres.AddRangeAsync(activeTheatre, inactiveTheatre);
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetActiveAsync();

        // Assert
        Assert.Single(results);
        Assert.Equal(activeTheatre.Id, results.First().Id);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnTheatreWithMatchingName()
    {
        // Arrange
        var theatre = Theatre.Create("Unique Theatre Name", "Some Address");
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNameAsync("Unique Theatre Name");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(theatre.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTheatreProperties()
    {
        // Arrange
        var theatre = Theatre.Create("Original Name", "Original Address");
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        // Act
        theatre.Name = "Updated Name";
        theatre.Address = "Updated Address";
        await _repository.UpdateAsync(theatre);

        // Assert
        var updatedTheatre = await _context.Theatres.FindAsync(theatre.Id);
        Assert.NotNull(updatedTheatre);
        Assert.Equal("Updated Name", updatedTheatre.Name);
        Assert.Equal("Updated Address", updatedTheatre.Address);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTheatreFromDatabase()
    {
        // Arrange
        var theatre = Theatre.Create("Theatre to Delete", "Some Address");
        await _context.Theatres.AddAsync(theatre);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(theatre.Id);

        // Assert
        var deletedTheatre = await _context.Theatres.FindAsync(theatre.Id);
        Assert.Null(deletedTheatre);
    }
}
