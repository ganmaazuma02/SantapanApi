using SantapanApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public interface IAccountService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string userName, string password, string firstName, string lastName, string role);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<UpgradeToCatererResult> UpgradeCustomerToCatererAsync(string email);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
    }
}
