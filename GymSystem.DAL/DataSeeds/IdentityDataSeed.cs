using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.DataSeeds
{
    public static class IdentityDataSeed
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            ILogger logger, CancellationToken ct=default)
        {
            try
            {
                bool hasRole = roleManager.Roles.Any();
                bool hasUser = userManager.Users.Any();
                if (hasRole && hasUser) return;

                if (!hasRole)
                {
                    var roles = new List<IdentityRole>()
                    {
                        new IdentityRole() {Name = "SuperAdmin"},
                        new IdentityRole() {Name = "Admin"}
                    };
                    foreach (var roleName in roles.Select(x => x.Name))
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                            if (!roleResult.Succeeded)
                                logger.LogError("Failed to create Role....");
                        }
                        
                    }
                }

                if (!hasUser)
                {
                    var mainUser = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Elgazar",
                        UserName = "mohady",
                        Email = "mohamed@gmail.com",
                        PhoneNumber = "01024026300",

                    };
                    var userResult = await userManager.CreateAsync(mainUser, "P@ssw0rd");
                    await userManager.AddToRoleAsync(mainUser, "SuperAdmin");
                    if (!userResult.Succeeded)
                    {
                        logger.LogError("Failed to seed User");
                        return;
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed To Seed Identity Data");
                throw;

            }
        }
    }
}
