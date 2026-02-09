namespace WroclawTheatreTickets.Infrastructure.Tests.Services;

using FakeItEasy;
using FakeItEasy.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;
using WroclawTheatreTickets.Domain.Common;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Services;
using Xunit;

public class TheatreRepertoireSyncServiceTests
{
    private readonly IRepertoireDataService _dataService;
    private readonly ITheatreProviderService _theatreProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<TheatreRepertoireSyncService> _logger;
    private readonly TheatreRepertoireSyncService _service;

    public TheatreRepertoireSyncServiceTests()
    {
        _dataService = A.Fake<IRepertoireDataService>();
        _theatreProvider = A.Fake<ITheatreProviderService>();
        _mediator = A.Fake<IMediator>();
        _logger = A.Fake<ILogger<TheatreRepertoireSyncService>>();

        _service = new TheatreRepertoireSyncService(_dataService, _theatreProvider, _mediator, _logger);
    }

    [Fact]
    public async Task SyncTheatreRepertoireAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var theatre = CreateTheatre();
        var shows = CreateValidShows(3, theatre.Id);

        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .Returns(theatre);

        A.CallTo(() => _dataService.FetchAndMapRepertoireAsync(
            theatre.Id, A<CancellationToken>._))
            .Returns(shows);

        A.CallTo(() => _mediator.Send(A<SaveOrUpdateShowCommand>._, A<CancellationToken>._))
            .Returns(Task.FromResult(Result<ShowDto>.Success(new ShowDto { Id = Guid.NewGuid() })));

        // Act
        var result = await _service.SyncTheatreRepertoireAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(3, result.TotalEventsProcessed);
        Assert.Equal(3, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);

        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _dataService.FetchAndMapRepertoireAsync(
            theatre.Id, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => _mediator.Send(
            A<SaveOrUpdateShowCommand>._, A<CancellationToken>._))
            .MustHaveHappened(3, Times.Exactly);
    }

    [Fact]
    public async Task SyncTheatreRepertoireAsync_WithNoShows_ReturnsZeroProcessed()
    {
        // Arrange
        var theatre = CreateTheatre();

        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .Returns(theatre);

        A.CallTo(() => _dataService.FetchAndMapRepertoireAsync(
            theatre.Id, A<CancellationToken>._))
            .Returns([]);

        // Act
        var result = await _service.SyncTheatreRepertoireAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(0, result.TotalEventsProcessed);
        Assert.Equal(0, result.SuccessCount);
        Assert.Equal(0, result.FailureCount);

        // Mediator should not be called at all
        A.CallTo(() => _mediator.Send(
            A<SaveOrUpdateShowCommand>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task SyncTheatreRepertoireAsync_WithTheatreProviderError_ReturnsError()
    {
        // Arrange
        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .Returns((Theatre?)null);

        // Act
        var result = await _service.SyncTheatreRepertoireAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Failed to get or create theatre", result.ErrorMessage);
        Assert.Equal(0, result.SuccessCount);
    }

    [Fact]
    public async Task SyncTheatreRepertoireAsync_WithDataServiceError_ReturnsError()
    {
        // Arrange
        var theatre = CreateTheatre();

        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .Returns(theatre);

        A.CallTo(() => _dataService.FetchAndMapRepertoireAsync(
            theatre.Id, A<CancellationToken>._))
            .Throws(new InvalidOperationException("API error"));

        // Act
        var result = await _service.SyncTheatreRepertoireAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Synchronization failed", result.ErrorMessage);
    }

    [Fact]
    public async Task SyncTheatreRepertoireAsync_WithPartialCommandFailures_CountsCorrectly()
    {
        // Arrange
        var theatre = CreateTheatre();
        var shows = CreateValidShows(4, theatre.Id);

        A.CallTo(() => _theatreProvider.GetOrCreateTheatreAsync(
            A<string>._, A<string>._, A<string>._))
            .Returns(theatre);

        A.CallTo(() => _dataService.FetchAndMapRepertoireAsync(
            theatre.Id, A<CancellationToken>._))
            .Returns(shows);

        // Two calls succeed, one fails, one throws exception
        var callCount = 0;
        A.CallTo(() => _mediator.Send(A<SaveOrUpdateShowCommand>._, A<CancellationToken>._))
            .ReturnsLazily(() =>
            {
                callCount++;
                if (callCount == 2)
                    return Task.FromResult(Result<ShowDto>.Failure("Command failed"));
                if (callCount == 4)
                    throw new InvalidOperationException("Processing error");
                return Task.FromResult(Result<ShowDto>.Success(new ShowDto { Id = Guid.NewGuid() }));
            });

        // Act
        var result = await _service.SyncTheatreRepertoireAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(4, result.TotalEventsProcessed);
        Assert.Equal(2, result.SuccessCount);
        Assert.Equal(2, result.FailureCount);
    }

    // Helper methods
    private Theatre CreateTheatre()
    {
        return Theatre.Create("Teatr Polski we Wrocławiu", "Wrocław, Poland");
    }

    private List<Show> CreateValidShows(int count, Guid theatreId)
    {
        var shows = new List<Show>();
        for (int i = 0; i < count; i++)
        {
            var show = Show.Create($"Show {i + 1}", theatreId, PerformanceType.Play, DateTime.Now.AddDays(i + 1));
            show.ExternalId = $"ext-{i + 1}";
            show.AgeRestriction = AgeRestriction.All;
            show.Language = "Polish";
            show.Duration = TimeSpan.FromMinutes(120);
            shows.Add(show);
        }
        return shows;
    }
}
