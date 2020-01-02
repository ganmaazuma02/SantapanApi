using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SantapanApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SantapanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly ApiInfo _apiInfo;

        public InfoController(IOptions<ApiInfo> apiInfoWrapper)
        {
            _apiInfo = apiInfoWrapper.Value;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public ActionResult<ApiInfo> GetInfo()
        {
            return _apiInfo;
        }
    }
}
