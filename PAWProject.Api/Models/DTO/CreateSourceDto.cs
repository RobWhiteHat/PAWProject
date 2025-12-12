namespace PAWProject.Api.Models.DTO
{
    public class CreateSourceDto
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public string? Description { get; set; }
        public string ComponentType { get; set; } = "REST";
        public bool RequiresSecret { get; set; } = false;

    }
}
