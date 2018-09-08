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
            List<Row<Account>> accounts;
            try
            {
                var dbUsers = await _accountCollection.GetAsync();
                accounts = dbUsers.ToList();
                accounts.ForEach(account =>
                {
                    account.Value.Password = string.Empty;
                    account.Value.PasswordSalt = string.Empty;
                });
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return Json(accounts);
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<ActionResult> Get(string id)
        {
            Account account;
            try
            {
                account = await _accountCollection.FindAsync(id);
                account.Password = string.Empty;
                account.PasswordSalt = string.Empty;
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Json(e));
            }

            return StatusCode((int)HttpStatusCode.OK, Json(account));
        }
    }
}