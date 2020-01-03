
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantapanApi.Configurations;
using SantapanApi.Data;
using SantapanApi.Domain;
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
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly JwtSettings jwtSettings;
        private readonly SantapanDbContext context;

        public DefaultAccountService(UserManager<IdentityUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            JwtSettings jwtSettings,
            SantapanDbContext context)
        {
            this.userManager = userManager;
            this.tokenValidationParameters = tokenValidationParameters;
            this.jwtSettings = jwtSettings;
            this.context = context;
        }

        public async Task<GetUserResult> GetUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return new GetUserResult()
                {
                    Success = false
                };

            return new GetUserResult()
            {
                Success = true,
                Email = user.Email,
                UserId = user.Id
            };
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

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
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
                Expires = DateTime.UtcNow.Add(jwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
    }
}
