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
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Dtos;
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
        public async Task<ActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            var usersQuery = userService.GetUsersQuery();
            var users = sieveProcessor.Apply(sieveModel, usersQuery).ToList();

            List<SantapanUserDto> userDtos = new List<SantapanUserDto>();

            foreach(var user in users)
            {
                var roles = await userService.GetRolesFromUserAsync(user);
                userDtos.Add(new SantapanUserDto
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