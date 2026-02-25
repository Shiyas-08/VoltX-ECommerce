using ECommerce.Models;

namespace ECommerce.Data
{
    public class DbSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            // Ensure Roles exist
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                   new Role { Name = "Admin" },
                   new Role { Name = "User" }
 
                );

                 context.SaveChanges();
            }

            // Check Admin user by RoleId
            if (!context.Users.Any(x => x.RoleId == 1))
            {
                context.Users.Add(new User
                {
                    Name = "Admin",
                    Email = "admin@site.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = 1,   //admin roleId
                    IsBlocked = false
                });

                context.SaveChanges();
            }
        }
    }
}
