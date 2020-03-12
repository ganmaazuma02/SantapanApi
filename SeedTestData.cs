using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SantapanApi.Data;
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Domain.Entities;
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

            await AddTestCategories(
                services.GetRequiredService<SantapanDbContext>());

            await AddTestData(
                services.GetRequiredService<SantapanDbContext>(),
                services.GetRequiredService<UserManager<SantapanUser>>());

        }

        public static async Task AddTestCategories(
            SantapanDbContext context)
        {
            if (context.Categories.Any())
                return;

            Category dessertCategory = new Category { Name = Categories.Dessert };
            Category buffetCategory = new Category { Name = Categories.Buffet };
            Category hiteaCategory = new Category { Name = Categories.HiTea };
            Category officeCategory = new Category { Name = Categories.Office };
            Category stationCategory = new Category { Name = Categories.Station };
            Category weddingCategory = new Category { Name = Categories.Wedding };
            Category birthdayCategory = new Category { Name = Categories.Birthday };

            await context.Categories.AddAsync(dessertCategory);
            await context.Categories.AddAsync(birthdayCategory);
            await context.Categories.AddAsync(buffetCategory);
            await context.Categories.AddAsync(hiteaCategory);
            await context.Categories.AddAsync(officeCategory);
            await context.Categories.AddAsync(stationCategory);
            await context.Categories.AddAsync(weddingCategory);

            await context.SaveChangesAsync();
        }

        public static async Task AddTestData(
            SantapanDbContext context,
            UserManager<SantapanUser> userManager)
        {
            if (context.Caterings.Any())
                return;



            var testCateringUser = await userManager.FindByNameAsync("caterer");

            var suriaAiskrimCatering = new Catering
            {
                Name = "Suria Aiskrim",
                Details = "Aiskrim pelbagai perisa! Lick it, love it!",
                UserId = testCateringUser.Id,
                CreatedAt = DateTime.UtcNow,
                SSM = "105015-K",
                CompanyName = "Suria Sejukbeku Sdn. Bhd.",
                Street1 = "No. 131, Jalan Anggerik 1/4",
                Street2 = "Bandar Amanjaya",
                Postcode = "08000",
                State = "Kedah",
                City = "Sungai Petani",
                Longitude = 100.542240,
                Latitude = 5.637992
            };

            suriaAiskrimCatering.CateringCategories = new List<CateringCategory> 
            { 
                new CateringCategory { Category = context.Categories.Single(c => c.Name == Categories.Dessert), Catering = suriaAiskrimCatering },
                new CateringCategory { Category = context.Categories.Single(c => c.Name == Categories.Station), Catering = suriaAiskrimCatering }
            };

            suriaAiskrimCatering.CateringUnavailabilities = new List<CateringUnavailability>
            {
                new CateringUnavailability { UnavailableDate = new DateTime(2020, 1, 30), Session = CateringSession.Morning, Catering = suriaAiskrimCatering }
            };

            suriaAiskrimCatering.Packages = new List<Package>
            {
                new Package 
                { 
                    Name = "Bronze Package - 450 kon", 
                    Catering = suriaAiskrimCatering, 
                    Description = "Pelbagai perisa", 
                    Price = 350,
                    Serves = "450 kon aiskrim",
                    ServicePresentation = "Seorang pelayan",
                    SetupTime = "30 minit",
                    PackageRequirements = new List<PackageRequirement>
                    {
                        new PackageRequirement { Name = "Palam elektrik"}
                    },
                    Menus = new List<Menu>
                    {
                        new Menu { Name = "Aiskrim pilihan 2 perisa (Vanilla/coklat/durian/strawberi/keladi)" }
                    },
                    PackageOptions = new List<PackageOption>
                    {
                        new PackageOption 
                        { 
                            Title = "Pilih perisa aiskrim (2 sahaja)",
                            OptionsType = PackageOptionsTypes.MultiSelect, 
                            PackageOptionItems = new List<PackageOptionItem>
                            { 
                                new PackageOptionItem { Name = "Vanilla" }, 
                                new PackageOptionItem { Name = "Coklat" }, 
                                new PackageOptionItem { Name = "Durian" }, 
                                new PackageOptionItem { Name = "Strawberi" }, 
                                new PackageOptionItem { Name = "Keladi" }
                            }
                        }
                    }
                }
            };


            await context.Caterings.AddAsync(suriaAiskrimCatering);

            var testCateringUser2 = await userManager.FindByNameAsync("caterer2");

            var pakChaqCatering = new Catering
            {
                Name = "Katering Pak Chaq",
                Details = "Berpengalaman lebih 7 tahun dalam bidang penyediaan makanan dengan tujuan utama memberikan layanan terbaik kepada pelanggan dengan konsep lengkap dan mewah dalam pakej perkahwinan dan pakej lain.",
                UserId = testCateringUser2.Id,
                CreatedAt = DateTime.UtcNow,
                SSM = "105016-K",
                CompanyName = "A tu Z Wedding House Sdn.Bhd.",
                Street1 = "2531, 2532, 2533, Kawasan S.P.T Perindustrian",
                Street2 = "Seberang, Jalan Jelawat, Seberang Jaya",
                Postcode = "13700",
                State = "Pulau Pinang",
                City = "Perai",
                Longitude = 100.395528,
                Latitude = 5.402835
            };

            pakChaqCatering.CateringCategories = new List<CateringCategory>
            {
                new CateringCategory { Category = context.Categories.Single(c => c.Name == Categories.Wedding), Catering = pakChaqCatering}
            };

            await context.Caterings.AddAsync(pakChaqCatering);

            var testCateringUser3 = await userManager.FindByNameAsync("caterer3");

            var kambingBakarCatering = new Catering()
            {
                Name = "Kambing Bakar Mak Minah",
                Details = "Stesen kambing bakar",
                UserId = testCateringUser3.Id,
                CreatedAt = DateTime.UtcNow,
                SSM = "105017-K",
                CompanyName = "Aminah Resources Sdn.Bhd.",
                Street1 = "485-487, Jalan Permatang Rawa",
                Street2 = " Bandar Perda",
                Postcode = "14000",
                State = "Pulau Pinang",
                City = "Bukit Mertajam",
                Longitude = 100.438167,
                Latitude = 5.366525
            };

            kambingBakarCatering.CateringCategories = new List<CateringCategory>
            {
                new CateringCategory { Category = context.Categories.Single(c => c.Name == Categories.Wedding), Catering = kambingBakarCatering}
            };

            await context.Caterings.AddAsync(kambingBakarCatering);

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
