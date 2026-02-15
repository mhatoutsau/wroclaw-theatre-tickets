namespace WroclawTheatreTickets.Application.Contracts.Dtos;

/// <summary>
/// Maps API response to domain Show entities
/// Handles conversion from external API format to internal domain model
/// </summary>
public static class ApiDtoMapper
{
    /// <summary>
    /// Parse additional properties JSON (if present) to extract metadata
    /// Example: {"finances_account":"501","status_pl":"Performance with English subtitles"}
    /// </summary>
    public static Dictionary<string, string> ParseAdditionalProps(string? additionalProps)
    {
        if (string.IsNullOrWhiteSpace(additionalProps))
            return [];

        try
        {
            // Use System.Text.Json to parse the JSON string
            var jsonDoc = System.Text.Json.JsonDocument.Parse(additionalProps);
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
    /// Determine performance type from category and additional properties
    /// </summary>
    public static Domain.Entities.PerformanceType DeterminePerformanceType(
        RepertoireEventDto dto)
    {
        var categoryTitle = dto.RepertoireCategories.FirstOrDefault()?.Title ?? string.Empty;
        var title = dto.Title.ToLower();

        // Map based on category or title patterns
        return categoryTitle switch
        {
            var c when c.Contains("opera", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("opera") => Domain.Entities.PerformanceType.Opera,
            var c when c.Contains("balet", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("balet") => Domain.Entities.PerformanceType.Ballet,
            var c when c.Contains("komedii", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("komedia") => Domain.Entities.PerformanceType.Comedy,
            var c when c.Contains("dramat", StringComparison.OrdinalIgnoreCase) || 
                      title.Contains("dramat") => Domain.Entities.PerformanceType.Drama,
            var c when c.Contains("musical", StringComparison.OrdinalIgnoreCase) => Domain.Entities.PerformanceType.Musical,
            var c when c.Contains("koncert", StringComparison.OrdinalIgnoreCase) => Domain.Entities.PerformanceType.Concert,
            _ => Domain.Entities.PerformanceType.Play
        };
    }

    /// <summary>
    /// Determine age restriction from AgeCategories or title analysis
    /// </summary>
    public static Domain.Entities.AgeRestriction DetermineAgeRestriction(
        RepertoireEventDto dto)
    {
        var ageTitle = dto.AgeCategories.FirstOrDefault()?.Title ?? string.Empty;

        return ageTitle switch
        {
            var a when a.Contains("18+") => Domain.Entities.AgeRestriction.Age18,
            var a when a.Contains("16+") => Domain.Entities.AgeRestriction.Age16,
            var a when a.Contains("12+") => Domain.Entities.AgeRestriction.Age12,
            var a when a.Contains("6+") => Domain.Entities.AgeRestriction.Age6,
            _ => Domain.Entities.AgeRestriction.All
        };
    }

    /// <summary>
    /// Extract coordinates from building address (basic pattern matching)
    /// Currently returns null - would need reverse geocoding or coordinates in API
    /// </summary>
    public static (double? Latitude, double? Longitude) ExtractCoordinates(
        string address)
    {
        // Placeholder for future geocoding integration
        // Currently returns null as API doesn't provide coordinates
        return (null, null);
    }
}
