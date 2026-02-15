namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class RepertoireEventDto
{
    [JsonPropertyName("repertoireEventId")]
    public string RepertoireEventId { get; set; } = string.Empty;
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("paymentUrl")]
    public string? PaymentUrl { get; set; }
    
    [JsonPropertyName("paymentDisabled")]
    public bool PaymentDisabled { get; set; }
    
    [JsonPropertyName("hiddenFromRepertoire")]
    public bool HiddenFromRepertoire { get; set; }
    
    [JsonPropertyName("additionalProps")]
    public string? AdditionalProps { get; set; }
    
    [JsonPropertyName("repertoireCategories")]
    public List<RepertoireCategoryDto> RepertoireCategories { get; set; } = [];
    
    [JsonPropertyName("ageCategories")]
    public List<AgeCategoryDto> AgeCategories { get; set; } = [];
    
    [JsonPropertyName("showEvent")]
    public ShowEventDto? ShowEvent { get; set; }
    
    [JsonPropertyName("stage")]
    public StageDto? Stage { get; set; }
}
