namespace PAWProject.Mvc.Models
{
    public class ArticleViewModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ComponentType { get; set; }
        public bool RequiresSecret { get; set; }
    }
}
