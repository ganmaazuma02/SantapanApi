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
using SantapanApi.Contracts.V1.Requests;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Domain;
using SantapanApi.Domain.Constants;
using SantapanApi.Domain.Entities;
using SantapanApi.Services;
using Sieve.Models;
using Sieve.Services;

namespace SantapanApi.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class CateringsController : ControllerBase
    {
        private readonly ICateringService cateringService;
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        private readonly SieveProcessor sieveProcessor;

        public CateringsController(
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
        /// Returns all caterings
        /// </summary>
        /// <response code="200">Returns all caterings</response>
        [HttpGet(ApiRoutes.Caterings.GetAll)]
        [ProducesResponseType(typeof(List<CateringResponse>), 200)]
        public ActionResult GetAll([FromQuery]SieveModel sieveModel)
        {
            var cateringsQuery = cateringService.GetCateringsQuery();
            var caterings = sieveProcessor.Apply(sieveModel, cateringsQuery).ToList();

            List<CateringResponse> cateringDtos = new List<CateringResponse>();

            foreach (Catering catering in caterings)
            {
                cateringDtos.Add(new CateringResponse
                {
                    Id = catering.Id,
                    Categories = catering.CateringCategories.Select(c => c.Category.Name).ToList(),
                    Details = catering.Details,
                    Name = catering.Name,
                    UserId = catering.UserId,
                    CreatedAt = catering.CreatedAt,
                    SSM = catering.SSM,
                    CompanyName = catering.CompanyName,
                    State = catering.State,
                    City =catering.City,
                    Street1 = catering.Street1,
                    Street2 = catering.Street2,
                    Latitude = catering.Latitude,
                    Longitude = catering.Longitude,
                    Postcode = catering.Postcode
                });
            }

            return Ok(cateringDtos);

        }

        /// <summary>
        /// Updates a catering
        /// </summary>
        /// <param name="cateringId"></param>
        /// <param name="request"></param>
        /// <response code="200">Updates a catering</response>
        /// <response code="404">Catering not found</response>
        [HttpPut(ApiRoutes.Caterings.Update)]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Caterer)]
        [ProducesResponseType(typeof(CateringResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult> Update([FromRoute] Guid cateringId, [FromBody] UpdateCateringRequest request)
        {
            foreach (string category in request.CateringCategories)
            {
                if (Categories.CategoriesList.Find(c => c == category) == null)
                    return BadRequest(new ApiErrorResponse()
                    {
                        ErrorType = "Bad Request",
                        StatusCode = 400,
                        Errors = new[] { "Category doesn't exist." }
                    });
            }


            var catering = new Catering()
            {
                Id = cateringId,
                Name = request.Name,
                Details = request.Details
            };

            var updated = await cateringService.UpdateCateringAsync(catering, request.CateringCategories);

            if (updated)
            {
                CateringResponse cateringDto = new CateringResponse
                {
                    Id = cateringId,
                    Name = request.Name,
                    Details = request.Details,
                    Categories = request.CateringCategories.ToList()
                };
                return Ok(cateringDto);
            }

            return NotFound(new ApiErrorResponse()
            {
                ErrorType = "Not Found",
                StatusCode = 404,
                Errors = new[] { "Catering not found." }
            });
        }

        /// <summary>
        /// Deletes a catering
        /// </summary>
        /// <param name="cateringId"></param>
        /// <response code="204">Deletes a catering</response>
        /// <response code="404">Catering not found</response>
        [HttpDelete(ApiRoutes.Caterings.Delete)]
        [Authorize(Roles = RoleName.Admin)]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult> Delete([FromRoute] Guid cateringId)
        {
            var deleted = await cateringService.DeleteCateringAsync(cateringId);

            if (deleted)
                return NoContent();

            return NotFound(new ApiErrorResponse()
            {
                ErrorType = "Not Found",
                StatusCode = 404,
                Errors = new[] { "Catering not found." }
            });
        }

        /// <summary>
        /// Returns a catering
        /// </summary>
        /// <param name="cateringId"></param>
        /// <response code="200">Returns a catering</response>
        /// <response code="404">Catering not found</response>
        [HttpGet(ApiRoutes.Caterings.Get)]
        [ProducesResponseType(typeof(CateringResponse), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        public async Task<ActionResult> Get([FromRoute] Guid cateringId)
        {
            var catering = await cateringService.GetCateringByIdAsync(cateringId);

            if (catering == null)
                return NotFound(new ApiErrorResponse()
                {
                    ErrorType = "Not Found",
                    StatusCode = 404,
                    Errors = new[] { "Catering not found." }
                });

            CateringResponse cateringDto = new CateringResponse
            {
                Id = catering.Id,
                Name = catering.Name,
                UserId = catering.UserId,
                CreatedAt = catering.CreatedAt,
                Details = catering.Details,
                Categories = catering.CateringCategories.Select(c => c.Category.Name).ToList()
            };

            return Ok(cateringDto);
        }

        /// <summary>
        /// Creates a catering
        /// </summary>
        /// <response code="201">Creates a catering</response>
        /// <response code="400">Unable to create a catering due to validation error</response>
        [HttpPost(ApiRoutes.Caterings.Create)]
        [Authorize(Roles = RoleName.Admin)]
        [ProducesResponseType(typeof(CreateCateringSuccessResponse), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        public async Task<ActionResult> Create([FromBody] CreateCateringRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse()
                {
                    ErrorType = "Bad Request",
                    StatusCode = 400,
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });

            var userResult = await userService.GetCatererOrAdminUserByEmailAsync(request.Email);

            if (!userResult.Success)
                return NotFound(new ApiErrorResponse()
                {
                    ErrorType = "Not Found",
                    StatusCode = 404,
                    Errors = userResult.Errors
                });


            foreach (string category in request.CateringCategories)
            {
                if (Categories.CategoriesList.Find(c => c == category) == null)
                    return BadRequest(new ApiErrorResponse()
                    {
                        ErrorType = "Bad Request",
                        StatusCode = 400,
                        Errors = new[] { "Category doesn't exist." }
                    });
            }

            var catering = new Catering() 
            { 
                Name = request.Name, 
                Details = request.Details,
                UserId = userResult.UserId
            };

            var created = await cateringService.CreateCateringAsync(catering, request.CateringCategories);

            if (!created)
                return BadRequest(new ApiErrorResponse()
                {
                    ErrorType = "Bad Request",
                    StatusCode = 400,
                    Errors = new[] { "Unable to create a catering." }
                });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Caterings.Get.Replace("{cateringId}", catering.Id.ToString());

            var response = new CreateCateringSuccessResponse() { Id = catering.Id };

            return Created(locationUrl, response);
        }


    
    }   
}