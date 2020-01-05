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
using SantapanApi.Domain;
using SantapanApi.Dtos;
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

        [HttpPost(ApiRoutes.Account.RegisterCustomer)]
        public async Task<ActionResult> RegisterCustomer([FromBody] UserRegistrationRequest request)
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

            var authResponse = await accountService.RegisterAsync(
                request.Email, 
                request.UserName, 
                request.Password,
                request.FirstName,
                request.LastName,
                RoleName.Customer);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });

            return BadRequest(new AuthFailedResponse() 
            { 
                Errors = authResponse.Errors
            });
            

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleName.Admin)]
        [HttpPost(ApiRoutes.Account.RegisterCaterer)]
        public async Task<ActionResult> RegisterCaterer([FromBody] UserRegistrationRequest request)
        {
            // validate response model state
            if (!ModelState.IsValid)
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

            var authResponse = await accountService.RegisterAsync(
                request.Email,
                request.UserName,
                request.Password,
                request.FirstName,
                request.LastName,
                RoleName.Caterer);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });

            return BadRequest(new AuthFailedResponse()
            {
                Errors = authResponse.Errors
            });


        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleName.Admin)]
        [HttpPost(ApiRoutes.Account.RegisterAdmin)]
        public async Task<ActionResult> RegisterAdmin([FromBody] UserRegistrationRequest request)
        {
            // validate response model state
            if (!ModelState.IsValid)
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

            var authResponse = await accountService.RegisterAsync(
                request.Email,
                request.UserName,
                request.Password,
                request.FirstName,
                request.LastName,
                RoleName.Admin);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });

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
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });

            return BadRequest(new AuthFailedResponse()
            {
                Errors = authResponse.Errors
            });

        }

        [HttpPost(ApiRoutes.Account.Refresh)]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {

            var authResponse = await accountService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (authResponse.Success)
                return Ok(new AuthSuccessResponse() { Token = authResponse.Token, RefreshToken = authResponse.RefreshToken });

            return BadRequest(new AuthFailedResponse()
            {
                Errors = authResponse.Errors
            });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Account.Me)]
        public async Task<ActionResult> Me()
        {
            var result = await accountService.GetUserByIdAsync(HttpContext.GetUserId());

            if (!result.Success)
                return NotFound();

            List<CateringDto> cateringDtos = new List<CateringDto>();

            foreach (Catering catering in result.Caterings)
            {
                cateringDtos.Add(new CateringDto
                {
                    Category = catering.Category,
                    Details = catering.Details,
                    Id = catering.Id,
                    Name = catering.Name,
                    UserId = catering.UserId
                });
            }

            return Ok(new GetUserResponse { 
                UserId = result.UserId,
                Caterings = cateringDtos,
                CreatedAt = result.CreatedAt,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Email = result.Email,
                UserName = result.UserName
            });

        }
    }
}