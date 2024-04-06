using FiksComService.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Application
{
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : IdentityDbContext<User, Role, int>(options)
    {
        public DbSet<Component> Components { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(
                new Role() { Id = 1, Name = "Client", ConcurrencyStamp = "1", NormalizedName = "CLIENT" },
                new Role() { Id = 2, Name = "Administrator", ConcurrencyStamp = "2", NormalizedName = "ADMINISTRATOR" });

            var hasher = new PasswordHasher<User>();

            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "Adminek",
                    Email = "adminTest@interia.pl",
                    LockoutEnabled = false,
                    PhoneNumber = "758675867",
                    PasswordHash = hasher.HashPassword(null, "Abcd!5555"),
                    SecurityStamp = Guid.NewGuid().ToString("D"),
                    NormalizedEmail = "ADMINTEST@INTERIA.PL",
                    NormalizedUserName = "ADMINEK",
                    EmailConfirmed = true
                });

            builder.Entity<IdentityUserRole<int>>().HasData(
                new IdentityUserRole<int>
                {
                    UserId = 1,
                    RoleId = 2
                });
            
            base.OnModelCreating(builder);
        }
    }
}
