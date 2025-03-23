using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using RandomString4Net;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var posts = await _context.Posts
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Bio = user.Bio,
                ImageName = user.ImageName,
                Posts = posts,
                SelectedSong = user.SelectedSong
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Lägg till random användarnamn om fältet lämnas tomt, samt string för tom bio
            string randomString = RandomString.GetString(Types.ALPHABET_LOWERCASE);
            string emptyBio = "Här var det tomt!";

            user.Name = model.Name ?? randomString;
            user.Bio = model.Bio ?? emptyBio;
            user.SelectedSong = model.SelectedSong;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                // Generera ett unikt filnamn
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                user.ImageName = fileName;
            }

            await _userManager.UpdateAsync(user);
            TempData["Profile"] = "Profil sparad!";
            return RedirectToAction("Index", "Profile");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.Users
                .Where(u => u.Id == id)
                .Select(u => new ProfileViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Bio = u.Bio,
                    ImageName = u.ImageName,
                    Posts = _context.Posts
                        .Where(p => p.UserId == u.Id)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToList(),
                    SelectedSong = u.SelectedSong
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return View(user);
        }

        public async Task<IActionResult> Members()
        {
            var users = await _userManager.Users
                .Select(u => new MemberViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    ImageName = u.ImageName
                })
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ResetProfilePicture()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.ImageName = "default.jpg";
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }
    }
}