using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Storage.API.Data;
using Storage.API.Models;
using Storage.API_CAN.Models;

namespace Storage.API_CAN.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/SeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                //roles for app
                var roles = new List<AppRole>
                {
                    new AppRole{Name = "Member"},
                    new AppRole{Name = "Admin"},
                    new AppRole{Name = "Moderator"},
                    new AppRole{Name = "VIP"}
                };
                
                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }
                foreach (var user in users)
                {
                    userManager.CreateAsync(user, "password").Wait();
                    userManager.AddToRoleAsync(user, "Member");
                }
                
                //admino acc 

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                var result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
                }

            }
        }
    }
}