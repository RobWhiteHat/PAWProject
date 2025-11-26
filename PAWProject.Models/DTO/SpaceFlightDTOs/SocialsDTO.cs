using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class SocialsDTO
    {
        [JsonPropertyName("x")]
        public string? X { get; set; }

        [JsonPropertyName("youtube")]
        public string? Youtube { get; set; }

        [JsonPropertyName("instagram")]
        public string? Instagram { get; set; }

        [JsonPropertyName("linkedin")]
        public string? Linkedin { get; set; }

        [JsonPropertyName("mastodon")]
        public string? Mastodon { get; set; }

        [JsonPropertyName("bluesky")]
        public string? Bluesky { get; set; }
    }
}