using Microsoft.EntityFrameworkCore;
using SantapanApi.Data;
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public class DefaultCateringService : ICateringService
    {
        private readonly SantapanDbContext context;

        public DefaultCateringService(SantapanDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CreateCateringAsync(Catering catering, string[] cateringCategories)
        {
            List<CateringCategory> cateringCategoriesForDb = new List<CateringCategory>();

            foreach (string cateringCategory in cateringCategories)
            {
                var category = context.Categories.Single(c => c.Name == cateringCategory);
                if (category == null)
                    return false;

                cateringCategoriesForDb.Add(new CateringCategory { Category = context.Categories.Single(c => c.Name == cateringCategory), Catering = catering });
            }

            catering.CateringCategories = cateringCategoriesForDb;

            await context.Caterings.AddAsync(catering);
            var created = await context.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> DeleteCateringAsync(Guid cateringId)
        {
            var catering = await GetCateringByIdAsync(cateringId);

            if (catering == null)
                return false;

            context.Caterings.Remove(catering);
            var deleted = await context.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<Catering> GetCateringByIdAsync(Guid cateringId)
        {
            return await context.Caterings.SingleOrDefaultAsync(c => c.Id == cateringId);
        }

        public IQueryable<Catering> GetCateringsQuery()
        {
            return context.Caterings.Include(c => c.CateringCategories).ThenInclude(c => c.Category).AsNoTracking();
        }

        public async Task<bool> UpdateCateringAsync(Catering cateringToUpdate)
        {
            context.Caterings.Update(cateringToUpdate);
            var updated = await context.SaveChangesAsync();
            return updated > 0;
        }
    }
}
