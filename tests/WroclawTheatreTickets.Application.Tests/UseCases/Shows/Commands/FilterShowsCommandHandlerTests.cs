namespace WroclawTheatreTickets.Application.Tests.UseCases.Shows.Commands;

using AutoMapper;
using FakeItEasy;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Repositories;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;
using WroclawTheatreTickets.Domain.Entities;
using Xunit;
using AutoFixture;

public class FilterShowsCommandHandlerTests
{
    private readonly IShowRepository _showRepository;
    private readonly IMapper _mapper;
    private readonly FilterShowsCommandHandler _handler;
    private readonly Fixture _fixture;

    public FilterShowsCommandHandlerTests()
    {
        _showRepository = A.Fake<IShowRepository>();
        _mapper = A.Fake<IMapper>();
        _handler = new FilterShowsCommandHandler(_showRepository, _mapper);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryAndMapper()
    {
        // Arrange
        var criteria = new ShowFilterCriteria
        {
            Type = "Play",
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(30)
        };
        var command = new FilterShowsCommand(criteria);

        var shows = new List<Show>
        {
            Show.Create("Test Show 1", Guid.NewGuid(), PerformanceType.Play, DateTime.UtcNow.AddDays(5)),
            Show.Create("Test Show 2", Guid.NewGuid(), PerformanceType.Play, DateTime.UtcNow.AddDays(10))
        };

        var showDtos = new List<ShowDto>
        {
            new ShowDto { Id = shows[0].Id, Title = shows[0].Title },
            new ShowDto { Id = shows[1].Id, Title = shows[1].Title }
        };

        A.CallTo(() => _showRepository.FilterAsync(criteria))
            .Returns(shows);
        A.CallTo(() => _mapper.Map<IEnumerable<ShowDto>>(shows))
            .Returns(showDtos);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        A.CallTo(() => _showRepository.FilterAsync(criteria))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _mapper.Map<IEnumerable<ShowDto>>(shows))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WithEmptyResults_ShouldReturnEmptyList()
    {
        // Arrange
        var criteria = new ShowFilterCriteria { Type = "NonExistentType" };
        var command = new FilterShowsCommand(criteria);

        A.CallTo(() => _showRepository.FilterAsync(criteria))
            .Returns(Enumerable.Empty<Show>());
        A.CallTo(() => _mapper.Map<IEnumerable<ShowDto>>(A<IEnumerable<Show>>._))
            .Returns(Enumerable.Empty<ShowDto>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithMultipleFilters_ShouldPassAllCriteriaToRepository()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var criteria = new ShowFilterCriteria
        {
            Type = "Musical",
            DateFrom = DateTime.UtcNow,
            DateTo = DateTime.UtcNow.AddDays(60),
            TheatreId = theatreId,
            PriceFrom = 50,
            PriceTo = 200,
            Language = "Polish",
            AgeRestriction = "Age12"
        };
        var command = new FilterShowsCommand(criteria);

        var shows = new List<Show>
        {
            Show.Create("Musical Show", theatreId, PerformanceType.Musical, DateTime.UtcNow.AddDays(15))
        };

        A.CallTo(() => _showRepository.FilterAsync(A<ShowFilterCriteria>._))
            .Returns(shows);
        A.CallTo(() => _mapper.Map<IEnumerable<ShowDto>>(A<IEnumerable<Show>>._))
            .Returns(new List<ShowDto> { new ShowDto() });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        A.CallTo(() => _showRepository.FilterAsync(A<ShowFilterCriteria>.That.Matches(c =>
            c.Type == criteria.Type &&
            c.TheatreId == criteria.TheatreId &&
            c.PriceFrom == criteria.PriceFrom &&
            c.PriceTo == criteria.PriceTo &&
            c.Language == criteria.Language &&
            c.AgeRestriction == criteria.AgeRestriction
        ))).MustHaveHappenedOnceExactly();
    }
}
