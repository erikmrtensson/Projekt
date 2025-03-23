using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
 
        public string? ImageName { get; set; } = "/images/default.jpg";

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string? SelectedSong { get; set; }
    }
}