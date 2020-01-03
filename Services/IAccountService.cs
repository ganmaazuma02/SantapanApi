using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface IAccountService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<GetUserResult> GetUserAsync(string userId);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
