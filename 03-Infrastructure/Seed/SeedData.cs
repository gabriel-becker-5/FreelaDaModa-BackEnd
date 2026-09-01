using _02_Application.Authorization;
using _02_Application.DTOs;
using _02_Application.Interfaces;
using _04_Domain.Entities.UserInfo;

namespace _03_Infrastructure.Seed
{
    public class SeedData
    {
        public static async Task Initializer(IRoleService roleService,
                                     IUserService userService)
        {
            // Create all roles if they don't exist
            for (int i = 0; i < Roles.roles.Length; i++)
            {
                await roleService.CreateRoleAsync(Roles.roles[i]);
            }

            // Create Initial User
            UserRegisterDto dto = new UserRegisterDto
            {
                Email = "admin@admin.com",
                Name = "Admin",
                Password = "123",
                BirthDate = DateTime.Today
            };

            User? user = await userService.CreateUserAsync(dto);

            // Assign the “Admin” role to the Initial User
            Role? roleAdmin = await roleService.GetRoleAsync(Roles.Admin);

            if (user != null && roleAdmin != null)
            {
                await userService.CreateUserRoleAsync(user, roleAdmin);
            }
        }
    }
}