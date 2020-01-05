using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SantapanApi.Data;
using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi
{
    public class SeedTestData
    {

        public static async Task InitializeAsync(IServiceProvider services)
        {
            await AddTestUsers(
                services.GetRequiredService<RoleManager<SantapanRole>>(),
                services.GetRequiredService<UserManager<SantapanUser>>());

            await AddTestData(
                services.GetRequiredService<SantapanDbContext>(),
                services.GetRequiredService<UserManager<SantapanUser>>());
        }

        public static async Task AddTestData(
            SantapanDbContext context,
            UserManager<SantapanUser> userManager)
        {
            if (context.Caterings.Any())
                return;

            var testCateringUser = await userManager.FindByNameAsync("caterer");

            await context.Caterings.AddAsync(new Catering()
            {
                Name = "Suria Aiskrim",
                Category = Categories.Dessert,
                Details = "Aiskrim pelbagai perisa! Lick it, love it!",
                UserId = testCateringUser.Id
            });

            await context.SaveChangesAsync();
        }

        private static async Task AddTestUsers(
            RoleManager<SantapanRole> roleManager,
            UserManager<SantapanUser> userManager)
        {
            var dataExists = roleManager.Roles.Any() || userManager.Users.Any();
            if (dataExists)
            {
                return;
            }

            await roleManager.CreateAsync(new SantapanRole(RoleName.Admin));
            await roleManager.CreateAsync(new SantapanRole(RoleName.Caterer));
            await roleManager.CreateAsync(new SantapanRole(RoleName.Customer));

            var adminUser = new SantapanUser
            {
                Email = "admin@santapan.my",
                UserName = "admin",
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(adminUser, "@Abc123");
            await userManager.AddToRoleAsync(adminUser, RoleName.Admin);
            await userManager.UpdateAsync(adminUser);

            var catererUser = new SantapanUser
            {
                Email = "caterer@santapan.my",
                UserName = "caterer",
                FirstName = "Caterer",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(catererUser, "@Abc123");
            await userManager.AddToRoleAsync(catererUser, RoleName.Caterer);
            await userManager.UpdateAsync(catererUser);

            var customerUser = new SantapanUser
            {
                Email = "customer@santapan.my",
                UserName = "customer",
                FirstName = "Customer",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(customerUser, "@Abc123");
            await userManager.AddToRoleAsync(customerUser, RoleName.Customer);
            await userManager.UpdateAsync(customerUser);


        }
    }
}
