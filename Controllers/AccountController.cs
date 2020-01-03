using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.Requests;
using SantapanApi.Contracts.Responses;
using SantapanApi.Models;
using SantapanApi.Services;

namespace SantapanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            // validate response model state
            if(!ModelState.IsValid)
                return BadRequest(new AuthFailedResponse()
                {
                    Errors = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage))
                });


            // check for confirmation password
            if (request.Password != request.ConfirmPassword)
                return BadRequest(new AuthFailedResponse()
                {
                    Errors = new[] { "Password and confirmation password do not match." }
                });

            var authResponse = await accountService.RegisterAsync(request.Email, request.Password);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token });

            return BadRequest(new AuthFailedResponse() 
            { 
                Errors = authResponse.Errors
            });
            

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserLoginRequest request)
        {
            // validate response model state
            if (!ModelState.IsValid)
                return BadRequest(new AuthFailedResponse()
                {
                    Errors = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage))
                });

            var authResponse = await accountService.LoginAsync(request.Email, request.Password);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token });

            return BadRequest(new AuthFailedResponse()
            {
                Errors = authResponse.Errors
            });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("me")]
        public async Task<ActionResult> Me()
        {
            //var user = User.Identity.Name;

            return Ok(User.Identity.Name);

        }
    }
}