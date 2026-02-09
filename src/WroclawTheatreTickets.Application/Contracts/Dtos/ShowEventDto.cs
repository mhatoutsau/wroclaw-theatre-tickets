namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class ShowEventDto
{
    [JsonPropertyName("showEventId")]
    public string ShowEventId { get; set; } = string.Empty;
    
    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("shared")]
    public bool Shared { get; set; }
}
