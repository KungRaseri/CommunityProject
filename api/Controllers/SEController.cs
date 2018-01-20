using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Data.Models.StreamElements.Booties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/se/points")]
    [Authorize]
    public class SEPointsController : BaseApiController
    {
        private readonly StreamElements _seClient;

        public SEPointsController(IConfiguration configuration) : base(configuration)
        {
            _seClient = new StreamElements(Settings);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int)HttpStatusCode.OK, Json(topPoints));
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
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int)HttpStatusCode.OK, Json(topAllTimePoints));
        }
    }
}
