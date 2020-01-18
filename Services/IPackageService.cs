using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface IPackageService
    {
        IQueryable<Package> GetPackagesFromOneCateringQuery(Guid cateringId);

        Task<bool> CreatePackageAsync(Package package);
        Task<bool> UpdatePackageAsync(Package packageToUpdate, Guid cateringId);
        Task<Package> GetPackageByIdAsync(Guid packageId);
        Task<bool> DeletePackageAsync(Guid packageId);
    }
}
