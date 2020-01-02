using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SantapanApi.Contracts.Requests;
using SantapanApi.Contracts.Responses;
using SantapanApi.Models;

namespace SantapanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly JwtSettings jwtSettings;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            JwtSettings jwtSettings)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtSettings = jwtSettings;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            // check for confirmation password
            if (request.Password != request.ConfirmPassword)
                return BadRequest(new AuthFailedResponse()
                {
                    Errors = new[] { "Password and confirmation password do not match." }
                });

            // Register a new user
            var user = new IdentityUser()
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if(result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
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

                return Ok(new AuthSuccessResponse() { Token = tokenHandler.WriteToken(token) });
            }

            return BadRequest(new AuthFailedResponse() 
            { 
                Errors = result.Errors.Select(e => e.Description)
            });
            

        }
    }
}