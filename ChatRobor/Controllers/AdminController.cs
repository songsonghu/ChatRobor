using ChatRobor.Data;
using ChatRobor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatRobor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return Json(new { success = false, error = "Role name is required" });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, error = "User not found" });

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, error = "User not found" });

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Json(new { success = true });

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }
    }
}
