using Auth_API.Data;
using Auth_API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auth_API.Services
{
    public class ContextSeedService
    {
        private readonly UserDbContext userDbContext;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ContextSeedService(UserDbContext userDbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userDbContext = userDbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if(userDbContext.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0) 
            { 
                await userDbContext.Database.MigrateAsync();
            }

            if(!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole { Name = SD.AdminRole });
                await roleManager.CreateAsync(new IdentityRole { Name = SD.ManagerRole });
                await roleManager.CreateAsync(new IdentityRole { Name = SD.PlayerRole });
            }

            if(!userManager.Users.AnyAsync().GetAwaiter().GetResult())
            {
                var admin = new User
                {
                    FirstName = "admin",
                    LastName = "jackson",
                    UserName = SD.AdminUserName,
                    Email = SD.AdminUserName,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "123456");
                await userManager.AddToRolesAsync(admin, new[] { SD.AdminRole, SD.ManagerRole, SD.PlayerRole });
                await userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Surname, admin.LastName)
                });

                var manager = new User
                {
                    FirstName = "manager",
                    LastName = "wilson",
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(manager, "123456");
                await userManager.AddToRoleAsync(manager, SD.ManagerRole);
                await userManager.AddClaimsAsync(manager, new Claim[]
                {
                    new Claim(ClaimTypes.Email, manager.Email),
                    new Claim(ClaimTypes.Surname, manager.LastName)
                });

                var player = new User
                {
                    FirstName = "player",
                    LastName = "miller",
                    UserName = "player@example.com",
                    Email = "player@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(player, "123456");
                await userManager.AddToRoleAsync(player, SD.PlayerRole);
                await userManager.AddClaimsAsync(player, new Claim[]
                {
                    new Claim(ClaimTypes.Email, player.Email),
                    new Claim(ClaimTypes.Surname, player.LastName)
                });

                var vipplayer = new User
                {
                    FirstName = "vipplayer",
                    LastName = "tompson",
                    UserName = "vipplayer@example.com",
                    Email = "vipplayer@example.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(vipplayer, "123456");
                await userManager.AddToRoleAsync(vipplayer, SD.PlayerRole);
                await userManager.AddClaimsAsync(vipplayer, new Claim[]
                {
                    new Claim(ClaimTypes.Email, vipplayer.Email),
                    new Claim(ClaimTypes.Surname, vipplayer.LastName)
                });
            }
        }
    }
}
