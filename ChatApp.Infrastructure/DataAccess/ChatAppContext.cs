using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ChatApp.Infrastructure.DataAccess
{
    public class ChatAppContext : IdentityDbContext<AppUser>
    {
        public ChatAppContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<UserChatRoom> UserChatRooms { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserChatRoom>()
                .HasKey(ucr => new { ucr.UserId, ucr.ChatRoomId });
            builder.Entity<AppUser>()
                .HasMany(a => a.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId);
          
        }
    }
}
