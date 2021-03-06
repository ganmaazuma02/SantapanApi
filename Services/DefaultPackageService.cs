﻿using Microsoft.EntityFrameworkCore;
using SantapanApi.Data;
using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public class DefaultPackageService : IPackageService
    {
        private readonly SantapanDbContext context;

        public DefaultPackageService(SantapanDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreatePackageAsync(Package package)
        {
            await context.Packages.AddAsync(package);
            var created = await context.SaveChangesAsync();
            return created > 0;
        }

        public async Task<Package> GetPackageByIdAsync(Guid packageId)
        {
            return await context.Packages
                .Include(p => p.Menus)
                .Include(p => p.PackageOptions)
                .ThenInclude(o => o.PackageOptionItems)
                .Include(p => p.PackageRequirements)
                .SingleOrDefaultAsync(p => p.Id == packageId);
        }

        public IQueryable<Package> GetPackagesFromOneCateringQuery(Guid cateringId)
        {
            //return context.Caterings.Include(c => c.Packages).Single(c => c.Id == cateringId).Packages.AsQueryable();
            return context.Packages
                .Include(p => p.Catering)
                .Include(p => p.Menus)
                .Include(p => p.PackageOptions)
                .ThenInclude(o => o.PackageOptionItems)
                .Include(p => p.PackageRequirements)
                .Where(p => p.CateringId == cateringId);
        }

        public async Task<bool> DeletePackageAsync(Guid packageId)
        {
            var package = await GetPackageByIdAsync(packageId);

            if (package == null)
                return false;

            context.Packages.Remove(package);
            var deleted = await context.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<bool> UpdatePackageAsync(Package packageToUpdate, Guid cateringId)
        {
            packageToUpdate.CateringId = cateringId;

            context.Packages.Update(packageToUpdate);
            var updated = await context.SaveChangesAsync();
            return updated > 0;
        }
    }
}
