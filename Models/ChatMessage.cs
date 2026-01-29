namespace ChatRobor.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Role { get; set; } = "user"; // "user" or "assistant"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? TokenCount { get; set; }

        // Navigation properties
        public ChatConversation? Conversation { get; set; }
    }
}
