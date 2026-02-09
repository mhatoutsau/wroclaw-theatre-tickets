namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class AgeCategoryDto
{
    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
