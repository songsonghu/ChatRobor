namespace ChatRobor.Models
{
    public class ChatConversation
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = "New Conversation";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public ICollection<ChatMessage>? Messages { get; set; }
    }
}
