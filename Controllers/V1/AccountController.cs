using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Contracts.V1.Requests;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Extensions;
using SantapanApi.Services;

namespace SantapanApi.V1.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost(ApiRoutes.Account.Register)]
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

        [HttpPost(ApiRoutes.Account.Login)]
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
        [HttpGet(ApiRoutes.Account.Me)]
        public async Task<ActionResult> Me()
        {
            var result = await accountService.GetUserAsync(HttpContext.GetUserId());

            if (!result.Success)
                return NotFound();

            return Ok(result);

        }
    }
}