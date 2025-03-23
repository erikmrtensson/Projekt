using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Data;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.User) 
                .OrderByDescending(p => p.CreatedAt) 
                .ToListAsync();

            return View(posts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Rensa inmatning för nytt inlägg
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Inlägget får inte vara tomt!";
                return RedirectToAction("Index");
            }

            var post = new Post { 
                Content = content.Trim(), 
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                TempData["Post"] = "Inlägget raderat";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (post.UserId != userId) return Forbid();

            TempData["Post"] = "Inlägget raderat";
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}