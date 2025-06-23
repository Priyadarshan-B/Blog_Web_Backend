namespace Blog_Web_Backend.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string? UserId { get; set; }
        public string AuthorName { get; set; } = "";
        public string Preferences { get; set; } = "";
        public string? ImageUrl { get; set; }
    }

}