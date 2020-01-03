using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface IAccountService
    {
        public Task<AuthenticationResult> RegisterAsync(string email, string password);
        public Task<AuthenticationResult> LoginAsync(string email, string password);
        public Task<GetUserResult> GetUserAsync(string userId);
    }
}
