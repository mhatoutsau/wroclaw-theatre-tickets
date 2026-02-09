namespace WroclawTheatreTickets.Infrastructure.Services;

using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Domain.Entities;

/// <summary>
/// Maps Teatr Polski API responses to domain Show entities.
/// Handles conversion from external API format (RepertoireEventDto) to internal domain model (Show).
/// This mapper is specific to Teatr Polski API structure and should only be used by TeatrPolskiRepertoireDataService.
/// </summary>
public static class TeatrPolskiApiDtoMapper
{
    /// <summary>
    /// Maps a Teatr Polski API event to a Show domain entity.
    /// Validates required fields and performs all necessary transformations.
    /// </summary>
    /// <param name="dto">The API event DTO</param>
    /// <param name="theatreId">ID of the theatre for this show</param>
    /// <returns>Mapped Show entity, or null if required fields are missing</returns>
    public static Show? MapDtoToShowEntity(RepertoireEventDto dto, Guid theatreId)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(dto.Title) || dto.Stage?.Building == null)
            return null;

        var performanceType = DeterminePerformanceType(dto);
        var ageRestriction = DetermineAgeRestriction(dto);
        var additionalProps = ParseAdditionalProps(dto.AdditionalProps);

        // Extract language information from additional props if available
        var language = additionalProps.ContainsKey("status_pl") &&
            additionalProps["status_pl"].Contains("English", StringComparison.OrdinalIgnoreCase)
            ? "English"
            : "Polish";

        var show = Show.Create(dto.Title, theatreId, performanceType, dto.Date);
        show.ExternalId = dto.RepertoireEventId;
        show.Duration = TimeSpan.FromMinutes(dto.Duration);
        show.AgeRestriction = ageRestriction;
        show.Language = language;
        show.TicketUrl = dto.PaymentUrl;
        show.IsActive = !dto.PaymentDisabled;

        return show;
    }

    /// <summary>
    /// Parse additional properties JSON (if present) to extract metadata.
    /// Example: {"finances_account":"501","status_pl":"Performance with English subtitles"}
    /// </summary>
    public static Dictionary<string, string> ParseAdditionalProps(string? additionalProps)
    {
        if (string.IsNullOrWhiteSpace(additionalProps))
            return [];

        try
        {
            // Use System.Text.Json to parse the JSON string
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(additionalProps);
            var result = new Dictionary<string, string>();

            foreach (var prop in jsonDoc.RootElement.EnumerateObject())
            {
                result[prop.Name] = prop.Value.GetString() ?? string.Empty;
            }

            return result;
        }
        catch
        {
            // If parsing fails, return empty dict
            return [];
        }
    }

    /// <summary>
    /// Determine performance type from category and additional properties.
    /// </summary>
    public static PerformanceType DeterminePerformanceType(RepertoireEventDto dto)
    {
        var categoryTitle = dto.RepertoireCategories.FirstOrDefault()?.Title ?? string.Empty;
        var title = dto.Title.ToLower();

        // Map based on category or title patterns
        return categoryTitle switch
        {
            var c when c.Contains("opera", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("opera") => PerformanceType.Opera,
            var c when c.Contains("balet", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("balet") => PerformanceType.Ballet,
            var c when c.Contains("komedii", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("komedia") => PerformanceType.Comedy,
            var c when c.Contains("dramat", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("dramat") => PerformanceType.Drama,
            var c when c.Contains("musical", StringComparison.OrdinalIgnoreCase) => PerformanceType.Musical,
            var c when c.Contains("koncert", StringComparison.OrdinalIgnoreCase) => PerformanceType.Concert,
            _ => PerformanceType.Play
        };
    }

    /// <summary>
    /// Determine age restriction from AgeCategories or title analysis.
    /// </summary>
    public static AgeRestriction DetermineAgeRestriction(RepertoireEventDto dto)
    {
        var ageTitle = dto.AgeCategories.FirstOrDefault()?.Title ?? string.Empty;

        return ageTitle switch
        {
            var a when a.Contains("18+") => AgeRestriction.Age18,
            var a when a.Contains("16+") => AgeRestriction.Age16,
            var a when a.Contains("12+") => AgeRestriction.Age12,
            var a when a.Contains("6+") => AgeRestriction.Age6,
            _ => AgeRestriction.All
        };
    }
}
