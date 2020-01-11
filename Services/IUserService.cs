using SantapanApi.Domain;
using SantapanApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface IUserService
    {
        Task<GetUserResult> GetUserByIdAsync(string userId);
        Task<GetUserResult> GetCatererOrAdminUserByEmailAsync(string email);
        IQueryable<SantapanUser> GetUsersQuery();
        Task<IList<string>> GetRolesFromUserAsync(SantapanUser user);
    }
}
