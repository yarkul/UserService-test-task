using Microsoft.EntityFrameworkCore;
using UserServiceTestProject.DbContexts.DbModels;

namespace UserServiceTestProject.DbContexts
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UserDb");
            }
        }
    }
}