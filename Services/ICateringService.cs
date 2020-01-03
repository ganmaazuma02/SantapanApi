using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface ICateringService
    {
        Task<List<Catering>> GetCateringsAsync();
        Task<Catering> GetCateringByIdAsync(Guid cateringId);
        Task<bool> CreateCateringAsync(Catering catering);
        Task<bool> UpdateCateringAsync(Catering cateringToUpdate);
        Task<bool> DeleteCateringAsync(Guid cateringId);
    }
}
