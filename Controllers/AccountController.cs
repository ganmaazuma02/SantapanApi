using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Dtos;

namespace SantapanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Check if the model is valid
            if(ModelState.IsValid)
            {
                // Register a new user
                var user = new IdentityUser()
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email
                };

                var result = await userManager.CreateAsync(user, registerDto.Password);

                if(result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(result);
                }

                return BadRequest(result.Errors);
            }


            return BadRequest();
        }
    }
}