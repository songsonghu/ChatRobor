namespace ChatRobor.Models
{
    public class UserPreference
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Model { get; set; } = "deepseek-chat";
        public float Temperature { get; set; } = 0.7f;
        public int MaxTokens { get; set; } = 2048;
        public bool ShowTimestamp { get; set; } = true;
        public bool EnableNotifications { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser? User { get; set; }
    }
}
