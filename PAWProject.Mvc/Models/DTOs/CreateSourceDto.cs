using System.ComponentModel.DataAnnotations;

namespace PAWProject.Mvc.Models.DTOs
{
    public class CreateSourceDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = "";

        [Required]
        [Url]
        [StringLength(1000)]
        public string Url { get; set; } = "";

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(100)]
        public string ComponentType { get; set; } = "REST";

        public bool RequiresSecret { get; set; } = false;

    }
}
