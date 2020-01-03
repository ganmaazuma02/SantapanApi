using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SantapanApi.Contracts.V1;
using SantapanApi.Contracts.V1.Requests;
using SantapanApi.Contracts.V1.Responses;
using SantapanApi.Domain;
using SantapanApi.Services;

namespace SantapanApi.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CateringsController : ControllerBase
    {
        private readonly ICateringService cateringService;

        public CateringsController(ICateringService cateringService)
        {
            this.cateringService = cateringService;
        }

        [HttpGet(ApiRoutes.Caterings.GetAll)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await cateringService.GetCateringsAsync());
        }

        [HttpPut(ApiRoutes.Caterings.Update)]
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

            return NotFound();
        }

        [HttpDelete(ApiRoutes.Caterings.Delete)]
        public async Task<ActionResult> Delete([FromRoute] Guid cateringId)
        {
            var deleted = await cateringService.DeleteCateringAsync(cateringId);

            if (deleted)
                return NoContent();

            return NotFound();
        }

        [HttpGet(ApiRoutes.Caterings.Get)]
        public async Task<ActionResult> Get([FromRoute] Guid cateringId)
        {
            var catering = await cateringService.GetCateringByIdAsync(cateringId);

            if (catering == null)
                return NotFound();

            return Ok(catering);
        }

        [HttpPost(ApiRoutes.Caterings.Create)]
        public async Task<ActionResult> Create([FromBody] CreateCateringRequest request)
        {
            var catering = new Catering() { Name = request.Name, Details = request.Details };

            var created = await cateringService.CreateCateringAsync(catering);

            if (!created)
                return BadRequest();

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Caterings.Get.Replace("{cateringId}", catering.Id.ToString());

            var response = new CreateCateringSuccessResponse() { Id = catering.Id };

            return Created(locationUrl, response);
        }
    }
}