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
using SantapanApi.Domain.Constants;
using SantapanApi.Domain.Entities;
using SantapanApi.Extensions;
using SantapanApi.Services;

namespace SantapanApi.V1.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
        private readonly ICateringService cateringService;
        private readonly IUserService userService;

        public AccountController(
            IAccountService accountService, 
            ICateringService cateringService,
            IUserService userService)
        {
            this.accountService = accountService;
            this.cateringService = cateringService;
            this.userService = userService;
        }

        /// <summary>
        /// Registers a customer user
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Registers a customer user</response>
        /// <response code="400">Unable to register a user due to validation error</response>
        [HttpPost(ApiRoutes.Account.RegisterCustomer)]
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
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

        /// <summary>
        /// Registers a caterer user
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Registers a caterer user</response>
        /// <response code="400">Unable to register a user due to validation error</response>
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
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

        /// <summary>
        /// Upgrades a customer user to a caterer and creates a catering
        /// </summary>
        /// <param name="request"></param>
        /// <response code="201">Upgrades a customer user to a caterer and creates a catering</response>
        /// <response code="400">Unable to upgrade the user due to validation error</response>
        [ProducesResponseType(typeof(UpgradeToCatererResponse), 201)]
        [ProducesResponseType(typeof(CreateCateringFailedResponse), 400)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleName.Admin)]
        [HttpPost(ApiRoutes.Account.Upgrade)]
        public async Task<ActionResult> UpgadeToCaterer([FromBody] UpgradeToCatererRequest request)
        {
            // validate response model state
            if (!ModelState.IsValid)
                return BadRequest(new CreateCateringFailedResponse()
                {
                    Errors = ModelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage))
                });

            foreach(string category in request.CateringCategories)
            {
                if (Categories.CategoriesList.Find(c => c == category) == null)
                    return BadRequest(new CreateCateringFailedResponse()
                    {
                        Errors = new[] { "Category doesn't exist." }
                    });
            } 

            var upgradeResult = await accountService.UpgradeCustomerToCatererAsync(request.Email);

            if (!upgradeResult.Success)
                return NotFound(new ResourceNotFoundResponse()
                {
                    Errors = upgradeResult.Errors
                });

            var catering = new Catering()
            {
                Name = request.CateringName,
                Details = request.CateringDetails,
                UserId = upgradeResult.UserId
            };

            var created = await cateringService.CreateCateringAsync(catering, request.CateringCategories);

            if (!created)
                return BadRequest(new CreateCateringFailedResponse()
                {
                    Errors = new[] { "Unable to create a catering." }
                });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Caterings.Get.Replace("{cateringId}", catering.Id.ToString());

            var response = new UpgradeToCatererResponse() { CateringId = catering.Id, UserId = upgradeResult.UserId};

            return Created(locationUrl, response);


        }

        /// <summary>
        /// Registers an admin user
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Registers an admin user</response>
        /// <response code="400">Unable to register the user due to validation error</response>
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
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

        /// <summary>
        /// Logs in the user
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Logs in the user</response>
        /// <response code="400">Unable to log the user in due to validation error</response>
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
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

        /// <summary>
        /// Returns a refresh token for JWT
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Returns a refresh token for JWT</response>
        /// <response code="400">Unable to retrieve the token due to validation error</response>
        [ProducesResponseType(typeof(AuthSuccessResponse), 200)]
        [ProducesResponseType(typeof(AuthFailedResponse), 400)]
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

        /// <summary>
        /// Returns the currently logged in user
        /// </summary>
        /// <response code="200">Returns the currently logged in user</response>
        /// <response code="404">Unable to find the user</response>
        [ProducesResponseType(typeof(GetUserResponse), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiRoutes.Account.Me)]
        public async Task<ActionResult> Me()
        {
            var result = await userService.GetUserByIdAsync(HttpContext.GetUserId());

            if (!result.Success)
                return NotFound();

            List<CateringResponse> cateringDtos = new List<CateringResponse>();

            foreach (Catering catering in result.Caterings)
            {
                cateringDtos.Add(new CateringResponse
                {
                    Categories = catering.CateringCategories.Select(c => c.Category.Name).ToList(),
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