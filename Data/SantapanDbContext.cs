using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Data
{
    public class SantapanDbContext : IdentityDbContext<SantapanUser, SantapanRole, string>
    {
        public SantapanDbContext(DbContextOptions<SantapanDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CateringCategory>()
                .HasKey(cc => new { cc.CateringId, cc.CategoryId });
            modelBuilder.Entity<CateringCategory>()
                .HasOne(bc => bc.Catering)
                .WithMany(b => b.CateringCategories)
                .HasForeignKey(bc => bc.CateringId);
            modelBuilder.Entity<CateringCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.CateringCategories)
                .HasForeignKey(bc => bc.CategoryId);
        }

        public DbSet<Catering> Caterings { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageOption> PackageOptions { get; set; }
        public DbSet<PackageOptionItem> PackageOptionItems { get; set; }
        public DbSet<PackageRequirement> PackageRequirements { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CateringUnavailability> CateringUnavailabilities { get; set; }
        public DbSet<CateringCategory> CateringCategories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
