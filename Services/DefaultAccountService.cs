
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantapanApi.Configurations;
using SantapanApi.Data;
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Domain.Entities;
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
        private readonly UserManager<SantapanUser> userManager;
        private readonly RoleManager<SantapanRole> roleManager;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly JwtSettings jwtSettings;
        private readonly SantapanDbContext context;

        public DefaultAccountService(UserManager<SantapanUser> userManager,
            RoleManager<SantapanRole> roleManager,
            TokenValidationParameters tokenValidationParameters,
            JwtSettings jwtSettings,
            SantapanDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenValidationParameters = tokenValidationParameters;
            this.jwtSettings = jwtSettings;
            this.context = context;
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
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string username, string password, string firstName, string lastName, string role)
        {

            // Register a new user
            var user = new SantapanUser()
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new AuthenticationResult() { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addRoleResult.Succeeded)
            {
                return new AuthenticationResult() { Success = false, Errors = addRoleResult.Errors.Select(e => e.Description) };
            }

            var updateUserResult = await userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                return new AuthenticationResult() { Success = false, Errors = updateUserResult.Errors.Select(e => e.Description) };
            }

            return await GenerateAuthenticationResultForUserAsync(user);

        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
                return new AuthenticationResult() { Errors = new string[] { "Invalid Token." } };

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);
                //.Subtract(jwtSettings.TokenLifeTime);

            if(expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResult() { Errors = new string[] { "This JWT has not expired yet." } };

            var jti = validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await context.RefreshTokens.SingleOrDefaultAsync(r => r.Token.ToString() == refreshToken);

            if(storedRefreshToken == null)
                return new AuthenticationResult() { Errors = new string[] { "This refresh token does not exist." } };

            if(DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResult() { Errors = new string[] { "This refresh token has expired." } };

            if(storedRefreshToken.Invalidated)
                return new AuthenticationResult() { Errors = new string[] { "This refresh token has been invalidated." } };

            if (storedRefreshToken.Used)
                return new AuthenticationResult() { Errors = new string[] { "This refresh token has been used." } };

            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResult() { Errors = new string[] { "This refresh token does not match this JWT." } };

            storedRefreshToken.Used = true;
            context.RefreshTokens.Update(storedRefreshToken);
            await context.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(validatedToken.Claims.Single(c => c.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);

        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //var tokenValidationParameters = this.tokenValidationParameters.Clone();
                //tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(SantapanUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(jwtSettings.TokenLifeTime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            return new AuthenticationResult() 
            { 
                Success = true, 
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };
        }

        public async Task<UpgradeToCatererResult> UpgradeCustomerToCatererAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
                return new UpgradeToCatererResult()
                {
                    Success = false,
                    Errors = new[] { "User does not exist." }
                };

            if (await userManager.IsInRoleAsync(user, RoleName.Admin))
                return new UpgradeToCatererResult()
                {
                    Success = false,
                    Errors = new[] { "User is an admin. No need for upgrading." }
                };

            if (await userManager.IsInRoleAsync(user, RoleName.Caterer))
                return new UpgradeToCatererResult()
                {
                    Success = false,
                    Errors = new[] { "User is a caterer. No need for upgrading." }
                };

            await userManager.AddToRoleAsync(user, RoleName.Caterer);
            await userManager.RemoveFromRoleAsync(user, RoleName.Customer);
            await userManager.UpdateAsync(user);

            return new UpgradeToCatererResult()
            {
                Success = true,
                UserId = user.Id
            };
        }
    }
}
