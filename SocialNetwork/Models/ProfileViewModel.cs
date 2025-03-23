namespace SocialNetwork.Models
{
    public class ProfileViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? ImageName { get; set; }
        public List<Post>? Posts { get; set; } 
        public string? SelectedSong { get; set; }
    }
}