using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class GatewayController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return StatusCode((int)HttpStatusCode.OK, Json("KungRaseri API is running..."));
        }
    }
}