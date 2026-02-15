namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class RepertoireCategoryDto
{
    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
