using System.ComponentModel.DataAnnotations;

namespace PAWProject.Mvc.Models
{
    public class SourceItemUploadViewModel
    {
        public int SourceId { get; set; }

        [Required]
        public IFormFile JsonFile { get; set; } = default!;
    }
}