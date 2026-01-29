using Microsoft.AspNetCore.Identity;

namespace ChatRobor.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Theme { get; set; } = "light";
        public string? Language { get; set; } = "zh-CN";

        // Navigation properties
        public ICollection<ChatConversation>? Conversations { get; set; }
        public ICollection<UserPreference>? Preferences { get; set; }
    }
}
