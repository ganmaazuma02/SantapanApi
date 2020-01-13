using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Domain.Entities;
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
    public class PackagesController : ControllerBase
    {
        private readonly IPackageService packageService;
        private readonly SieveProcessor sieveProcessor;

        public PackagesController(
            IPackageService packageService,
            SieveProcessor sieveProcessor)
        {
            this.packageService = packageService;
            this.sieveProcessor = sieveProcessor;
        }


        /// <summary>
        /// Returns all packages for one catering
        /// </summary>
        /// <response code="200">Returns all packages for one catering</response>
        [HttpGet(ApiRoutes.Packages.GetPackagesForOneCatering)]
        [ProducesResponseType(typeof(List<PackageResponse>), 200)]
        public ActionResult GetPackagesFromOneCatering([FromQuery]SieveModel sieveModel, [FromRoute] Guid cateringId)
        {
            var packagesQuery = packageService.GetPackagesFromOneCateringQuery(cateringId);
            var packages = sieveProcessor.Apply(sieveModel, packagesQuery).ToList();

            List<PackageResponse> packageDto = new List<PackageResponse>();

            foreach(Package package in packages)
            {
                packageDto.Add(new PackageResponse
                {
                    Id = package.Id,
                    Name = package.Name,
                    Description = package.Description,
                    Serves = package.Serves,
                    Price = package.Price,
                    ServicePresentation = package.ServicePresentation,
                    SetupTime = package.SetupTime,
                    Menus = package.Menus.Select(m => m.Name).ToArray(),
                    PackageOptions = package.PackageOptions.Select(p => new PackageOptionResponse 
                    { 
                        Id = p.Id,
                        OptionsType = p.OptionsType,
                        PackageOptionItems = p.PackageOptionItems.Select(pi => pi.Name).ToArray(),
                        Title = p.Title
                    }),
                    PackageRequirements = package.PackageRequirements.Select(p => p.Name).ToArray()
                });
            }

            return Ok(packageDto);
        }
    }
}
