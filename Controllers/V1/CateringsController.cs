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
using SantapanApi.Dtos;
using SantapanApi.Services;

namespace SantapanApi.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CateringsController : ControllerBase
    {
        private readonly ICateringService cateringService;
        private readonly IAccountService accountService;

        public CateringsController(
            ICateringService cateringService, 
            IAccountService accountService)
        {
            this.cateringService = cateringService;
            this.accountService = accountService;
        }

        [HttpGet(ApiRoutes.Caterings.GetAll)]
        public async Task<ActionResult> GetAll()
        {
            var caterings = await cateringService.GetCateringsAsync();
            List<CateringDto> cateringDtos = new List<CateringDto>();

            foreach(Catering catering in caterings)
            {
                cateringDtos.Add(new CateringDto
                {
                    Id = catering.Id,
                    Category = catering.Category,
                    Details = catering.Details,
                    Name = catering.Name,
                    UserId = catering.UserId
                });
            }

            return Ok(cateringDtos);
        }

        [HttpPut(ApiRoutes.Caterings.Update)]
        [Authorize(Roles = RoleName.Admin + "," + RoleName.Caterer)]
        public async Task<ActionResult> Update([FromRoute] Guid cateringId, [FromBody] UpdateCateringRequest request)
        {
            var catering = new Catering()
            {
                Id = cateringId,
                Name = request.Name,
                Details = request.Details
            };

            var updated = await cateringService.UpdateCateringAsync(catering);

            if (updated)
                return Ok(catering);

            return NotFound(new ResourceNotFoundResponse()
            {
                Errors = new[] { "Catering not found." }
            });
        }

        [HttpDelete(ApiRoutes.Caterings.Delete)]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> Delete([FromRoute] Guid cateringId)
        {
            var deleted = await cateringService.DeleteCateringAsync(cateringId);

            if (deleted)
                return NoContent();

            return NotFound(new ResourceNotFoundResponse()
            {
                Errors = new[] { "Catering not found." }
            });
        }

        [HttpGet(ApiRoutes.Caterings.Get)]
        public async Task<ActionResult> Get([FromRoute] Guid cateringId)
        {
            var catering = await cateringService.GetCateringByIdAsync(cateringId);

            if (catering == null)
                return NotFound(new ResourceNotFoundResponse()
                {
                    Errors = new[] { "Catering not found." }
                });

            return Ok(catering);
        }

        [HttpPost(ApiRoutes.Caterings.Create)]
        [Authorize(Roles = RoleName.Admin)]
        public async Task<ActionResult> Create([FromBody] CreateCateringRequest request)
        {
            var userResult = await accountService.GetCatererOrAdminUserByEmailAsync(request.Email);

            if (!userResult.Success)
                return NotFound(new ResourceNotFoundResponse()
                {
                    Errors = userResult.Errors
                });

            if (request.Category != Categories.Dessert
                && request.Category != Categories.MainCourse
                && request.Category != Categories.Side)
                return BadRequest(new CreateCateringFailedResponse()
                {
                    Errors = new[] { "Category doesn't exist." }
                });


            var catering = new Catering() 
            { 
                Name = request.Name, 
                Details = request.Details,
                Category = request.Category,
                UserId = userResult.UserId
            };

            var created = await cateringService.CreateCateringAsync(catering);

            if (!created)
                return BadRequest(new CreateCateringFailedResponse()
                {
                    Errors = new[] { "Category doesn't exist." }
                });

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Caterings.Get.Replace("{cateringId}", catering.Id.ToString());

            var response = new CreateCateringSuccessResponse() { Id = catering.Id };

            return Created(locationUrl, response);
        }
    }
}