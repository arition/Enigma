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
        public DbSet<Message> Message { get; set; }
        public DbSet<EncryptedData> EncryptedData { get; set; }
    }
}