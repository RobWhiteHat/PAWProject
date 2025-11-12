using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class ArticleDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("authors")]
        public List<AuthorDTO>? Authors { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("news_site")]
        public string? NewsSite { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("featured")]
        public bool Featured { get; set; }

        [JsonPropertyName("launches")]
        public List<LaunchDTO>? Launches { get; set; }

        [JsonPropertyName("events")]
        public List<EventDTO>? Events { get; set; }
    }
}
