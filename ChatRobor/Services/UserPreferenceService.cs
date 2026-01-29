using ChatRobor.Data;
using ChatRobor.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatRobor.Services
{
    public interface IUserPreferenceService
    {
        Task<UserPreference> GetUserPreferenceAsync(string userId);
        Task UpdateUserPreferenceAsync(string userId, UserPreference preference);
    }

    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly ApplicationDbContext _context;

        public UserPreferenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPreference> GetUserPreferenceAsync(string userId)
        {
            var preference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (preference == null)
            {
                preference = new UserPreference { UserId = userId };
                _context.UserPreferences.Add(preference);
                await _context.SaveChangesAsync();
            }

            return preference;
        }

        public async Task UpdateUserPreferenceAsync(string userId, UserPreference preference)
        {
            var existingPreference = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingPreference != null)
            {
                existingPreference.Model = preference.Model;
                existingPreference.Temperature = preference.Temperature;
                existingPreference.MaxTokens = preference.MaxTokens;
                existingPreference.ShowTimestamp = preference.ShowTimestamp;
                existingPreference.EnableNotifications = preference.EnableNotifications;
                existingPreference.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
