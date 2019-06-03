using EnigmaLib.Model;
using Microsoft.EntityFrameworkCore;

namespace EnigmaServer.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupInviteLink> GroupInviteLink { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<EncryptedData> EncryptedData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupUser>()
                .HasOne(t => t.Group)
                .WithMany(t => t.GroupUsers);

            modelBuilder.Entity<GroupUser>()
                .HasOne(t => t.User)
                .WithMany(t => t.GroupUsers);

            base.OnModelCreating(modelBuilder);
        }
    }
}