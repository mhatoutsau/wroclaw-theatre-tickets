namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class BuildingDto
{
    [JsonPropertyName("buildingId")]
    public string BuildingId { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
