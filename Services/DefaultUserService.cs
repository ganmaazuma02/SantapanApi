using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SantapanApi.Data;
using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public class DefaultUserService : IUserService
    {
        private readonly UserManager<SantapanUser> userManager;
        private readonly RoleManager<SantapanRole> roleManager;
        private readonly SantapanDbContext context;

        public DefaultUserService(UserManager<SantapanUser> userManager,
            RoleManager<SantapanRole> roleManager,
            SantapanDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public async Task<GetUserResult> GetUserByIdAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return new GetUserResult()
                {
                    Success = false
                };

            List<Catering> userCaterings = context.Caterings.Where(c => c.UserId == user.Id).ToList();

            return new GetUserResult()
            {
                Success = true,
                Email = user.Email,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Caterings = userCaterings,
                CreatedAt = user.CreatedAt,
                UserName = user.UserName
            };
        }

        public async Task<GetUserResult> GetCatererOrAdminUserByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return new GetUserResult()
                {
                    Success = false,
                    Errors = new[] { "User does not exist." }
                };

            if (!await userManager.IsInRoleAsync(user, RoleName.Caterer) && !await userManager.IsInRoleAsync(user, RoleName.Admin))
                return new GetUserResult()
                {
                    Success = false,
                    Errors = new[] { "User is not an admin nor a caterer." }
                };

            return new GetUserResult()
            {
                Success = true,
                Email = user.Email,
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Caterings = user.Caterings,
                CreatedAt = user.CreatedAt,
                UserName = user.UserName
            };
        }

        public IQueryable<SantapanUser> GetUsersQuery()
        {
            return context.Users.AsNoTracking();
        }

        public async Task<IList<string>> GetRolesFromUserAsync(SantapanUser user)
        {
            return await userManager.GetRolesAsync(user);
        }
    }
}
