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
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(a => a.Messages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId);

            builder.Entity<UserGroup>()
               .HasOne(ucr => ucr.User)
               .WithMany()
               .HasForeignKey(ucr => ucr.UserId)
               .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<UserGroup>()
                .HasOne(ucr => ucr.Group)
                .WithMany(cr => cr.UserGroups)
                .HasForeignKey(ucr => ucr.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FriendShip>()
                .HasOne(fs => fs.Requester)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fs => fs.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FriendShip>()
                .HasOne(fs => fs.Addressee)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fs => fs.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
