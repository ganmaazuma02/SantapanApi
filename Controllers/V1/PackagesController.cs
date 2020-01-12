using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Dtos;
using SantapanApi.Services;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class PackagesController
    {
        private readonly ICateringService cateringService;
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        private readonly SieveProcessor sieveProcessor;

        public PackagesController(
            ICateringService cateringService,
            IAccountService accountService,
            IUserService userService,
            SieveProcessor sieveProcessor)
        {
            this.cateringService = cateringService;
            this.accountService = accountService;
            this.userService = userService;
            this.sieveProcessor = sieveProcessor;
        }


        /// <summary>
        /// Returns all packages for one catering
        /// </summary>
        /// <response code="200">Returns all packages for one catering</response>
        [HttpGet(ApiRoutes.Packages.GetPackagesForOneCatering)]
        [ProducesResponseType(typeof(List<PackageDto>), 200)]
        public ActionResult GetPackagesForOneCatering([FromQuery]SieveModel sieveModel, [FromRoute] Guid cateringId)
        {

            throw new NotImplementedException();
        }
    }
}
