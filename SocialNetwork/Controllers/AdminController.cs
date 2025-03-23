using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers;

public class AdminController : Controller
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users.ToListAsync();
        return View(users);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && user.UserName != "admin@example.com")
        {

            // Radera poster från användaren
            var userPosts = _context.Posts.Where(p => p.UserId == user.Id);
            _context.Posts.RemoveRange(userPosts);
            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Användaren har raderats.";
            }
            else
            {
                TempData["ErrorMessage"] = "Ett fel uppstod vid borttagning.";
            }
        }
        return RedirectToAction("Users");
    }
}