using System;
using System.Net;
using System.Threading.Tasks;
using Data.Models.StreamElements.Booties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

namespace Api.Controllers.ThirdParty
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/se/points")]
    [Authorize]
    public class SEPointsController : BaseApiController
    {
        private readonly StreamElementsService _seClient;

        public SEPointsController(IConfiguration configuration) : base(configuration)
        {
            _seClient = new StreamElementsService(_settings);
        }

        [HttpGet]
        [Route("top")]
        public async Task<ActionResult> GetTopPoints()
        {
            GetTopBootiesResponse topPoints;

            try
            {
                topPoints = await _seClient.GetTopPoints();
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int) HttpStatusCode.OK, Json(topPoints));
        }

        [HttpGet]
        [Route("top/alltime")]
        public async Task<ActionResult> GetTopAlltimePoints()
        {
            GetTopBootiesResponse topAllTimePoints;

            try
            {
                topAllTimePoints = await _seClient.GetAllTimePoints();
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int) HttpStatusCode.OK, Json(topAllTimePoints));
        }
    }
}