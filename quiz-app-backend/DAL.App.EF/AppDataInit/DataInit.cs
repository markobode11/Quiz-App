using System;
using Domain.App.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.App.EF.AppDataInit
{
    public static class DataInit
    {
        public static void DropDatabase(AppDbContext ctx, ILogger logger)
        {
            logger.LogInformation("DropDatabase");
            ctx.Database.EnsureDeleted();
        }

        public static void MigrateDatabase(AppDbContext ctx, ILogger logger)
        {
            logger.LogInformation("MigrateDatabase");
            ctx.Database.Migrate();
        }

        public static async void SeedAppData(AppDbContext ctx, UserManager<AppUser> userManager, ILogger logger)
        {
            await ctx.SaveChangesAsync();
        }
        
        public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            ILogger logger)
        {
            logger.LogInformation("SeedIdentity");
            foreach (var roleName in InitialData.Roles)
            {
                var role = roleManager.FindByNameAsync(roleName).Result;
                if (role == null)
                {
                    role = new AppRole
                    {
                        Name = roleName,
                    };

                    var result = roleManager.CreateAsync(role).Result;
                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Role creation failed: {roleName}");
                    }

                    logger.LogInformation("Seeded role {Role}", roleName);
                }
            }

            foreach (var userInfo in InitialData.Users)
            {
                var user = new AppUser
                {
                    Email = userInfo.name,
                    UserName = userInfo.name,
                    Firstname = userInfo.firstName,
                    Lastname = userInfo.lastName,
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, userInfo.password).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException($"User creation failed: {user.Email}");
                }

                logger.LogInformation("Seeded user {User}", userInfo.name);


                if (userInfo.role == null) continue;
                var roleResult = userManager.AddToRoleAsync(user, userInfo.role).Result;
                if (!roleResult.Succeeded)
                {
                    throw new ApplicationException($"Adding roles failed: {user.Email}, {userInfo.role}");
                }
            }
        }
    }
}