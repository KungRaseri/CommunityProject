using System.Net;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly CouchDbStore<User> _userCollection;

        public UserController(IConfiguration configuration) : base(configuration)
        {
            _userCollection = new CouchDbStore<User>(Settings.CouchDbUri);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var users = await _userCollection.GetAsync();
            return StatusCode((int)HttpStatusCode.OK, Json(users));
        }
    }
}
