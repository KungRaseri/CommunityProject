using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCouch;

namespace Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{versions:ApiVersion}/[controller]")]
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly CouchDbStore<Account> _accountCollection;

        public AccountController(IConfiguration configuration) : base(configuration)
        {
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            List<Row<Account>> users;
            try
            {
                var dbUsers = await _accountCollection.GetAsync();
                users = dbUsers.ToList();
                users.ForEach(user =>
                {
                    user.Value.Password = string.Empty;
                    user.Value.PasswordSalt = string.Empty;
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return Json(users);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Get(string id)
        {
            Account user;
            try
            {
                user = await _accountCollection.FindAsync(id);
                user.Password = string.Empty;
                user.PasswordSalt = string.Empty;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int)HttpStatusCode.OK, Json(user));
        }
    }
}