using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Contracts.V1.Requests;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Domain.Constants;
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
        private readonly ICateringService cateringService;
        private readonly IPackageService packageService;
        private readonly SieveProcessor sieveProcessor;

        public PackagesController(
            ICateringService cateringService,
            IPackageService packageService,
            SieveProcessor sieveProcessor)
        {
            this.cateringService = cateringService;
            this.packageService = packageService;
            this.sieveProcessor = sieveProcessor;
        }

        /// <summary>
        /// Returns a package
        /// </summary>
        /// <response code="200">Returns a package</response>
        /// <response code="404">Package doesn't exist</response>
        [HttpGet(ApiRoutes.Packages.Get)]
        [ProducesResponseType(typeof(PackageResponse), 200)]
        [ProducesResponseType(typeof(PackageResponse), 404)]
        public async Task<ActionResult> Get([FromRoute] Guid packageId)
        {
            var package = await packageService.GetPackageByIdAsync(packageId);
            
            if(package == null)
                return NotFound(new ApiErrorResponse()
                {
                    ErrorType = "Not Found",
                    StatusCode = 404,
                    Errors = new string[] { "Package doesn't exist" }
                });

            return Ok(new PackageResponse {
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


        /// <summary>
        /// Creates a package for a catering
        /// </summary>
        /// <response code="201">Creates a package for a catering</response>
        /// <response code="400">Unable to create a package due to validation error</response>
        /// <response code="404">Catering doesn't exist</response>
        [HttpPost(ApiRoutes.Packages.CreatePackageForOneCatering)]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Caterer)]
        [ProducesResponseType(typeof(CreatePackageSuccessResponse), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult> CreatePackageForOneCatering([FromBody] CreatePackageRequest request, [FromRoute] Guid cateringId)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse()
                {
                    ErrorType = "Bad Request",
                    StatusCode = 400,
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            var catering = await cateringService.GetCateringByIdAsync(cateringId);

            if (catering == null)
                return NotFound(new ApiErrorResponse()
                {
                    ErrorType = "Not Found",
                    StatusCode = 404,
                    Errors = new string[] { "Catering doesn't exist" }
                });

            var package = new Package
            {
                Name = request.Name,
                Serves = request.Serves,
                ServicePresentation = request.ServicePresentation,
                SetupTime = request.SetupTime,
                Description = request.Description,
                Price = request.Price,
                Menus = request.Menus.Select(m => new Menu { Name = m }).ToList(),
                Catering = catering,
                PackageOptions = request.PackageOptions.Select(o => new PackageOption 
                { 
                    OptionsType = o.OptionsType, 
                    Title = o.Title,
                    PackageOptionItems = o.PackageOptionItems.Select(i => new PackageOptionItem { Name = i}).ToList()
                }).ToList(),
                PackageRequirements = request.PackageRequirements.Select(r => new PackageRequirement { Name = r}).ToList()
            };

            var created = await packageService.CreatePackageAsync(package);

            if (!created)
                return BadRequest(new ApiErrorResponse()
                {
                    ErrorType = "Bad Request",
                    StatusCode = 400,
                    Errors = new[] { "Unable to create a package." }
                });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Packages.Get.Replace("{packageId}", package.Id.ToString());

            var response = new CreatePackageSuccessResponse() { Id = package.Id };

            return Created(locationUrl, response);
        }

        /// <summary>
        /// Deletes a package
        /// </summary>
        /// <param name="packageId"></param>
        /// <response code="204">Deletes a package</response>
        /// <response code="404">Package not found</response>
        [HttpDelete(ApiRoutes.Packages.Delete)]
        [Authorize(Roles = RoleName.Admin)]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult> Delete([FromRoute] Guid packageId)
        {
            var deleted = await packageService.DeletePackageAsync(packageId);

            if (deleted)
                return NoContent();

            return NotFound(new ApiErrorResponse()
            {
                ErrorType = "Not Found",
                StatusCode = 404,
                Errors = new[] { "Package not found." }
            });
        }
    }
}
