using System.ComponentModel.DataAnnotations.Schema;

namespace PAWProject.Api.Models.DTO
{
    public class SourceFromJsonDto
    {
        public string Url { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; }
        public string ComponentType { get; set; }
        public int RequiresSecret { get; set; }
    }
}