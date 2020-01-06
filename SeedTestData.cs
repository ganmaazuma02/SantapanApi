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
                UserId = testCateringUser.Id,
                CreatedAt = DateTime.UtcNow
            });

            var testCateringUser2 = await userManager.FindByNameAsync("caterer2");

            await context.Caterings.AddAsync(new Catering()
            {
                Name = "Nasi Tomato Pak Chaq",
                Category = Categories.MainCourse,
                Details = "Nasi tomato dengan ayam masak merah.",
                UserId = testCateringUser2.Id,
                CreatedAt = DateTime.UtcNow
            });

            var testCateringUser3 = await userManager.FindByNameAsync("caterer3");

            await context.Caterings.AddAsync(new Catering()
            {
                Name = "Kambing Bakar Mak Minah",
                Category = Categories.Side,
                Details = "Stesen kambing bakar",
                UserId = testCateringUser3.Id,
                CreatedAt = DateTime.UtcNow
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

            var catererUser2 = new SantapanUser
            {
                Email = "caterer2@santapan.my",
                UserName = "caterer2",
                FirstName = "Caterer2",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(catererUser2, "@Abc123");
            await userManager.AddToRoleAsync(catererUser2, RoleName.Caterer);
            await userManager.UpdateAsync(catererUser2);

            var catererUser3 = new SantapanUser
            {
                Email = "caterer3@santapan.my",
                UserName = "caterer3",
                FirstName = "Caterer3",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(catererUser3, "@Abc123");
            await userManager.AddToRoleAsync(catererUser3, RoleName.Caterer);
            await userManager.UpdateAsync(catererUser3);

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
