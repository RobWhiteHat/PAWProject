using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class EventDTO
    {
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }

        [JsonPropertyName("provider")]
        public string? Provider { get; set; }
    }
}