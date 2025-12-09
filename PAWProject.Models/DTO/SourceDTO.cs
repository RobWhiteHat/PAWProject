using System.Text.Json.Serialization;

namespace PAWProject.Data.Models
{
    public class SourceDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("componentType")]
        public string ComponentType { get; set; } = null!;

        [JsonPropertyName("requiresSecret")]
        public bool RequiresSecret { get; set; }
    }
}
