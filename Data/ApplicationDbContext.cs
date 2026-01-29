using ChatRobor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatRobor.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChatConversation> ChatConversations { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<UserPreference> UserPreferences { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ChatConversation configuration
            builder.Entity<ChatConversation>()
                .HasOne(c => c.User)
                .WithMany(u => u.Conversations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatConversation>()
                .Property(c => c.Title)
                .HasMaxLength(255);

            // ChatMessage configuration
            builder.Entity<ChatMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatMessage>()
                .Property(m => m.Content)
                .IsRequired();

            builder.Entity<ChatMessage>()
                .Property(m => m.Role)
                .HasMaxLength(20);

            // UserPreference configuration
            builder.Entity<UserPreference>()
                .HasOne(p => p.User)
                .WithMany(u => u.Preferences)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserPreference>()
                .Property(p => p.Model)
                .HasMaxLength(100);

            // Indexes
            builder.Entity<ChatConversation>()
                .HasIndex(c => c.UserId);

            builder.Entity<ChatMessage>()
                .HasIndex(m => m.ConversationId);

            builder.Entity<UserPreference>()
                .HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}
