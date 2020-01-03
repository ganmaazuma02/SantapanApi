using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SantapanApi.Configurations;
using SantapanApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SantapanApi.Services
{
    public class DefaultAccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> userManager;
        //private readonly SignInManager<IdentityUser> signInManager;
        private readonly JwtSettings jwtSettings;

        public DefaultAccountService(UserManager<IdentityUser> userManager,
            //SignInManager<IdentityUser> signInManager,
            JwtSettings jwtSettings)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.jwtSettings = jwtSettings;
        }

        public Task<GetUserResult> GetUserAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return new AuthenticationResult()
                {
                    Success = false,
                    Errors = new string[]
                    {
                        "User does not exists."
                    }
                };

            var userHasValidPassword = await userManager.CheckPasswordAsync(user, password);

            if(!userHasValidPassword)
                return new AuthenticationResult()
                {
                    Success = false,
                    Errors = new string[]
                    {
                        "Email/Password doesn't match."
                    }
                };

            //await signInManager.SignInAsync(user, isPersistent: false);
            return GenerateAuthenticationResultForUser(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {

            // Register a new user
            var user = new IdentityUser()
            {
                UserName = email,
                Email = email
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new AuthenticationResult() { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            //await signInManager.SignInAsync(user, isPersistent: false);
            return GenerateAuthenticationResultForUser(user);

        }

        private AuthenticationResult GenerateAuthenticationResultForUser(IdentityUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim("id", user.Id),

                    }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult() { Success = true, Token = tokenHandler.WriteToken(token) };
        }
    }
}
