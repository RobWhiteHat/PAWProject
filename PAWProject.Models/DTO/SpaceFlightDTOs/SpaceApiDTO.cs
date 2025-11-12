using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class SpaceApiDTO
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<ArticleDTO>? Results { get; set; }
    }
}
