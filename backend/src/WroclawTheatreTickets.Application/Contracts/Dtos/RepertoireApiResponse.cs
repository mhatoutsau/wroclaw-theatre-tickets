namespace WroclawTheatreTickets.Application.Contracts.Dtos;

using System.Text.Json.Serialization;

public class RepertoireApiResponse
{
    [JsonPropertyName("events")]
    public List<RepertoireEventDto> Events { get; set; } = [];
}
