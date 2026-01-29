using ChatRobor.Models;
using ChatRobor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatRobor.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingsController(
            IUserPreferenceService userPreferenceService,
            UserManager<ApplicationUser> userManager)
        {
            _userPreferenceService = userPreferenceService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.GetUserAsync(User);
            var preference = await _userPreferenceService.GetUserPreferenceAsync(userId);

            ViewBag.Preference = preference;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ApplicationUser model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Json(new { success = true, message = "Profile updated successfully" });

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePreferences(UserPreference model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            model.UserId = userId;
            await _userPreferenceService.UpdateUserPreferenceAsync(userId, model);

            return Json(new { success = true, message = "Preferences updated successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
                return Json(new { success = true, message = "Password changed successfully" });

            return Json(new { success = false, errors = result.Errors.Select(e => e.Description) });
        }
    }
}
