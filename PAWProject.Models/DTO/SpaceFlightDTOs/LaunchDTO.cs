using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class LaunchDTO
    {
        [JsonPropertyName("launch_id")]
        public Guid LaunchId { get; set; }

        [JsonPropertyName("provider")]
        public string? Provider { get; set; }
    }
}