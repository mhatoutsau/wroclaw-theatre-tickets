namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class StageDto
{
    [JsonPropertyName("stageId")]
    public string StageId { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("buildingId")]
    public string BuildingId { get; set; } = string.Empty;
    
    [JsonPropertyName("building")]
    public BuildingDto? Building { get; set; }
}
