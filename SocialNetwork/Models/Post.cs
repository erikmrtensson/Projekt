

namespace SocialNetwork.Models
{

    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
