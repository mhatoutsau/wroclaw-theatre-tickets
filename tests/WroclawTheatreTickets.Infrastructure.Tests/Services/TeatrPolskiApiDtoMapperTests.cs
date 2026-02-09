namespace WroclawTheatreTickets.Infrastructure.Tests.Services;

using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Domain.Entities;
using WroclawTheatreTickets.Infrastructure.Services;
using Xunit;

public class TeatrPolskiApiDtoMapperTests
{
    [Fact]
    public void MapDtoToShowEntity_WithValidDto_ReturnsShowEntity()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto(theatreName: "Test Opera", categoryTitle: "opera");

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.NotNull(show);
        Assert.Equal(dto.Title, show.Title);
        Assert.Equal(dto.RepertoireEventId, show.ExternalId);
        Assert.Equal(dto.Date, show.StartDateTime);
        Assert.Equal(theatreId, show.TheatreId);
        Assert.NotNull(show.Duration);
        Assert.Equal(TimeSpan.FromMinutes(dto.Duration), show.Duration);
    }

    [Fact]
    public void MapDtoToShowEntity_WithMissingTitle_ReturnsNull()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto();
        dto.Title = string.Empty;

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.Null(show);
    }

    [Fact]
    public void MapDtoToShowEntity_WithMissingStage_ReturnsNull()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto();
        dto.Stage = null;

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.Null(show);
    }

    [Fact]
    public void MapDtoToShowEntity_WithMissingStageBuilding_ReturnsNull()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto();
        dto.Stage!.Building = null;

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.Null(show);
    }

    [Fact]
    public void MapDtoToShowEntity_WithEnglishInAdditionalProps_SetsLanguageEnglish()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto();
        dto.AdditionalProps = """{"status_pl":"Performance with English subtitles"}""";

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.NotNull(show);
        Assert.Equal("English", show.Language);
    }

    [Fact]
    public void MapDtoToShowEntity_WithoutEnglishInAdditionalProps_SetsLanguagePolish()
    {
        // Arrange
        var theatreId = Guid.NewGuid();
        var dto = CreateValidRepertoireEventDto();
        dto.AdditionalProps = """{"status_pl":"Representation in Polish"}""";

        // Act
        var show = TeatrPolskiApiDtoMapper.MapDtoToShowEntity(dto, theatreId);

        // Assert
        Assert.NotNull(show);
        Assert.Equal("Polish", show.Language);
    }

    [Fact]
    public void DeterminePerformanceType_WithOperaCategory_ReturnsOpera()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "opera");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Opera, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithBalletCategory_ReturnsBallet()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "balet");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Ballet, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithComedyCategory_ReturnsComedy()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "komedii");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Comedy, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithDramaCategory_ReturnsDrama()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "dramat");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Drama, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithMusicalCategory_ReturnsMusical()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "musical");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Musical, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithConcertCategory_ReturnsConcert()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "koncert");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Concert, type);
    }

    [Fact]
    public void DeterminePerformanceType_WithUnknownCategory_ReturnsPlay()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(categoryTitle: "unknown");

        // Act
        var type = TeatrPolskiApiDtoMapper.DeterminePerformanceType(dto);

        // Assert
        Assert.Equal(PerformanceType.Play, type);
    }

    [Fact]
    public void DetermineAgeRestriction_WithAge18Category_ReturnsAge18()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(ageTitle: "18+");

        // Act
        var restriction = TeatrPolskiApiDtoMapper.DetermineAgeRestriction(dto);

        // Assert
        Assert.Equal(AgeRestriction.Age18, restriction);
    }

    [Fact]
    public void DetermineAgeRestriction_WithAge16Category_ReturnsAge16()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(ageTitle: "16+");

        // Act
        var restriction = TeatrPolskiApiDtoMapper.DetermineAgeRestriction(dto);

        // Assert
        Assert.Equal(AgeRestriction.Age16, restriction);
    }

    [Fact]
    public void DetermineAgeRestriction_WithAge12Category_ReturnsAge12()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(ageTitle: "12+");

        // Act
        var restriction = TeatrPolskiApiDtoMapper.DetermineAgeRestriction(dto);

        // Assert
        Assert.Equal(AgeRestriction.Age12, restriction);
    }

    [Fact]
    public void DetermineAgeRestriction_WithAge6Category_ReturnsAge6()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto(ageTitle: "6+");

        // Act
        var restriction = TeatrPolskiApiDtoMapper.DetermineAgeRestriction(dto);

        // Assert
        Assert.Equal(AgeRestriction.Age6, restriction);
    }

    [Fact]
    public void DetermineAgeRestriction_WithNoCategory_ReturnsAll()
    {
        // Arrange
        var dto = CreateValidRepertoireEventDto();
        dto.AgeCategories = [];

        // Act
        var restriction = TeatrPolskiApiDtoMapper.DetermineAgeRestriction(dto);

        // Assert
        Assert.Equal(AgeRestriction.All, restriction);
    }

    [Fact]
    public void ParseAdditionalProps_WithValidJson_ReturnsDictionary()
    {
        // Arrange
        var json = """{"finances_account":"501","status_pl":"Performance with English subtitles"}""";

        // Act
        var props = TeatrPolskiApiDtoMapper.ParseAdditionalProps(json);

        // Assert
        Assert.NotEmpty(props);
        Assert.Equal("501", props["finances_account"]);
        Assert.Contains("English", props["status_pl"]);
    }

    [Fact]
    public void ParseAdditionalProps_WithNullJson_ReturnsEmptyDictionary()
    {
        // Act
        var props = TeatrPolskiApiDtoMapper.ParseAdditionalProps(null);

        // Assert
        Assert.Empty(props);
    }

    [Fact]
    public void ParseAdditionalProps_WithEmptyJson_ReturnsEmptyDictionary()
    {
        // Act
        var props = TeatrPolskiApiDtoMapper.ParseAdditionalProps(string.Empty);

        // Assert
        Assert.Empty(props);
    }

    [Fact]
    public void ParseAdditionalProps_WithInvalidJson_ReturnsEmptyDictionary()
    {
        // Arrange
        var invalidJson = "{invalid json}";

        // Act
        var props = TeatrPolskiApiDtoMapper.ParseAdditionalProps(invalidJson);

        // Assert
        Assert.Empty(props);
    }

    // Helper method to create valid test DTOs
    private RepertoireEventDto CreateValidRepertoireEventDto(
        string theatreName = "Test Show",
        string categoryTitle = "play",
        string ageTitle = "0+")
    {
        return new RepertoireEventDto
        {
            RepertoireEventId = Guid.NewGuid().ToString(),
            Title = theatreName,
            Date = DateTime.Now.AddDays(7),
            Duration = 120,
            HiddenFromRepertoire = false,
            PaymentUrl = "https://example.com/tickets",
            PaymentDisabled = false,
            Stage = new StageDto
            {
                Building = new BuildingDto { Name = "Main Stage" }
            },
            RepertoireCategories =
            [
                new RepertoireCategoryDto { Title = categoryTitle }
            ],
            AgeCategories =
            [
                new AgeCategoryDto { Title = ageTitle }
            ],
            AdditionalProps = null
        };
    }
}
