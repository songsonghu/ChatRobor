using ChatRobor.Data;
using ChatRobor.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatRobor.Services
{
    public interface IChatService
    {
        Task<ChatConversation> CreateConversationAsync(string userId);
        Task<ChatMessage> AddMessageAsync(int conversationId, string content, string role);
        Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId);
        Task<ChatConversation?> GetConversationAsync(int id, string userId);
        Task<IEnumerable<ChatMessage>> GetConversationMessagesAsync(int conversationId);
        Task DeleteConversationAsync(int conversationId, string userId);
        Task UpdateConversationTitleAsync(int conversationId, string title, string userId);
    }

    public class ChatService : IChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatConversation> CreateConversationAsync(string userId)
        {
            var conversation = new ChatConversation
            {
                UserId = userId,
                Title = $"Conversation {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
            };

            _context.ChatConversations.Add(conversation);
            await _context.SaveChangesAsync();
            return conversation;
        }

        public async Task<ChatMessage> AddMessageAsync(int conversationId, string content, string role)
        {
            var message = new ChatMessage
            {
                ConversationId = conversationId,
                Content = content,
                Role = role
            };

            _context.ChatMessages.Add(message);

            // Update conversation's UpdatedAt timestamp
            var conversation = await _context.ChatConversations.FindAsync(conversationId);
            if (conversation != null)
            {
                conversation.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<ChatConversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.ChatConversations
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<ChatConversation?> GetConversationAsync(int id, string userId)
        {
            return await _context.ChatConversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && !c.IsDeleted);
        }

        public async Task<IEnumerable<ChatMessage>> GetConversationMessagesAsync(int conversationId)
        {
            return await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteConversationAsync(int conversationId, string userId)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

            if (conversation != null)
            {
                conversation.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateConversationTitleAsync(int conversationId, string title, string userId)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.UserId == userId);

            if (conversation != null)
            {
                conversation.Title = title;
                conversation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
