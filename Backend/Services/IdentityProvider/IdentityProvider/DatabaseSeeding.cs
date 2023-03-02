using Duende.IdentityServer.EntityFramework.DbContexts;
using IdentityModel;
using Infrastructure.EFCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IdentityProvider {
    internal static class DatabaseSeeding {
        public static async Task MigrateDatabases (this WebApplication app) {
            await app.MigrateDatabase<ApplicationDbContext>();
            await app.MigrateDatabase<PersistedGrantDbContext>();
        }

        public static async Task SeedDatabase (this WebApplication app) {
            await app.SeedDatabase<ApplicationDbContext>(async (context, services, logger) => {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await SeedRoles(roleManager, logger);
                await SeedUsers(userManager, roleManager, logger);
            });
        }

        private static async Task SeedRoles (RoleManager<IdentityRole> roleManager, ILogger<ApplicationDbContext> logger) {
            try {
                var roleCount = await roleManager.Roles.CountAsync();
                if (roleCount != 0) {
                    logger.LogInformation("Role seeding skipped");
                    return;
                }

                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                await roleManager.CreateAsync(new IdentityRole(Roles.User));

                logger.LogInformation("Roles seeding complete");
            } catch (Exception ex) {
                logger.LogError("Failed to seed roles", ex);
            }
        }

        private static async Task SeedUsers (UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ApplicationDbContext> logger) {
            try {
                var userCount = await userManager.Users.CountAsync();
                if (userCount != 0) {
                    logger.LogInformation("User seeding skipped");
                    return;
                }

                await CreateUserAsync(userManager,
                    new ApplicationUser() {
                        UserName = "user1",
                        Email = "user1@user.com",
                        EmailConfirmed = true,
                        Nickname = "User1"
                    },
                    "User123!",
                    Roles.User
                );

                logger.LogInformation("Users seeding complete");
            } catch (Exception ex) {
                logger.LogError("Failed to seed users", ex);
            }
        }

        private static async Task CreateUserAsync (UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role) {
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
            await userManager.AddClaimsAsync(user, new Claim[] {
                new Claim(JwtClaimTypes.Name, user.Nickname),
            });
        }

    }
}
