using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Services;
using Sieve.Models;
using Sieve.Services;

namespace SantapanApi.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleName.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly SieveProcessor sieveProcessor;

        public UsersController(
            IUserService userService, 
            SieveProcessor sieveProcessor)
        {
            this.userService = userService;
            this.sieveProcessor = sieveProcessor;
        }

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <response code="200">Returns all users</response>
        [HttpGet(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(List<SantapanUserResponse>), 200)]
        public async Task<ActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            var usersQuery = userService.GetUsersQuery();
            var users = sieveProcessor.Apply(sieveModel, usersQuery).ToList();

            List<SantapanUserResponse> userDtos = new List<SantapanUserResponse>();

            foreach(var user in users)
            {
                var roles = await userService.GetRolesFromUserAsync(user);
                userDtos.Add(new SantapanUserResponse
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    Email = user.Email,
                    Roles = roles,
                    UserId = user.Id
                });
            }

            return Ok(userDtos);
        }
    }
}