using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class GatewayController : Controller
    {
        [HttpGet]
        public ActionResult Get()
        {
            return Json("KungRaseri API is running...");
        }
    }
}