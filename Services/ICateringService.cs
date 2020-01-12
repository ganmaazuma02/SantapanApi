using SantapanApi.Domain;
using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface ICateringService
    {
        IQueryable<Catering> GetCateringsQuery();
        Task<Catering> GetCateringByIdAsync(Guid cateringId);
        Task<bool> CreateCateringAsync(Catering catering, string[] cateringCategories);
        Task<bool> UpdateCateringAsync(Catering cateringToUpdate, string[] cateringCategories);
        Task<bool> DeleteCateringAsync(Guid cateringId);
    }
}
