using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Data;
using SocialNetwork.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=socialnetwork.db"));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Post}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

// Roller och användare
// Todo: Lägg till att admin kan radera användare + inlägg
using (var scope = app.Services.CreateScope()) {
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Roller
    var roles = new[] {"Admin" };
    foreach(var role in roles) {
        if(!await roleManager.RoleExistsAsync(role)) {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Skapa admin
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    var users = new [] {
        new { Email = "admin123@admin.com", Password = "PASSword!23", Role = "Admin" }
    };

    foreach (var user in users) 
    {
        var identityUser = await userManager.FindByEmailAsync(user.Email);
        if (identityUser == null)
        {
            identityUser = new ApplicationUser { UserName = user.Email, Email = user.Email };
            await userManager.CreateAsync(identityUser, user.Password);

            // Tilldela roll
            await userManager.AddToRoleAsync(identityUser, user.Role);
        }
    }
}

app.Run();
