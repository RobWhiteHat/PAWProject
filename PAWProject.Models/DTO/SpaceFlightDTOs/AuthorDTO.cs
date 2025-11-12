using System.Text.Json.Serialization;

namespace PAWProject.Models.DTO.SpaceFlightDTOs
{
    public class AuthorDTO
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("socials")]
        public SocialsDTO? Socials { get; set; }
    }
}