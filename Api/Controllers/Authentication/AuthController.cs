using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers.Authentication
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly CouchDbStore<Account> _accountCollection;
        private readonly Crypto _cryptoService;

        public AuthController(IConfiguration configuration) : base(configuration)
        {
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);
            _cryptoService = new Crypto(_settings.CookieToken);
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> Register(Account account)
        {
            if (string.IsNullOrEmpty(account.Email) || string.IsNullOrEmpty(account.Password))
                return StatusCode((int)HttpStatusCode.BadRequest,
                    Json(new { error = "Email or Password are required" }));

            var dbAccount = await _accountCollection.FindAsync(account.Email, "account-email");

            if (dbAccount != null)
                return StatusCode((int)HttpStatusCode.BadRequest, Json(new { error = "Account already exists" }));

            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            var hashedPassword = _cryptoService.PasswordCrypt(account.Password, salt);

            account = new Account
            {
                Email = account.Email,
                Password = hashedPassword,
                PasswordSalt = salt
            };

            dbAccount = await _accountCollection.AddOrUpdateAsync(account);

            dbAccount.Password = string.Empty;
            dbAccount.PasswordSalt = string.Empty;

            return StatusCode((int)HttpStatusCode.OK, Json(dbAccount));
        }

        [Route("external/twitch/callback")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalCallback(string code, string scope, string state = "", string error = "", string error_description = "")
        {
            if (!string.IsNullOrEmpty(error))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, $"Error: {error} Description: {error_description}");
            }
            return StatusCode((int)HttpStatusCode.OK, "");
        }

        [Route("token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Token(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return StatusCode((int)HttpStatusCode.BadRequest,
                    Json(new { error = "Email or Password are required" }));

            var account = await _accountCollection.FindAsync(email, "account-email");

            if (account == null)
                return StatusCode((int)HttpStatusCode.Unauthorized, Json(new { error = "Invalid credentials" }));

            var hashedPassword = _cryptoService.PasswordCrypt(password, account.PasswordSalt);

            if (!hashedPassword.Equals(account.Password))
                return StatusCode((int)HttpStatusCode.Unauthorized, Json(new { error = "Invalid credentials" }));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, account._id),
                new Claim(JwtRegisteredClaimNames.Email, account.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Keys.JWTSecurityKey));

            var jwt = new JwtSecurityToken(new JwtHeader(
                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)),
                new JwtPayload("kungraseri-api", "kungraseri-audience", claims, null, DateTime.UtcNow.AddDays(1)));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            Token savedToken;

            try
            {
                var dbToken = await TokenCollection.FindAsync(account._id, "token-accountid");
                savedToken = await TokenCollection.AddOrUpdateAsync(
                    dbToken == null || dbToken.Expiration < DateTime.UtcNow
                        ? new Token
                        {
                            Value = token,
                            UserId = account._id,
                            Issued = DateTime.UtcNow,
                            Expiration = jwt.ValidTo
                        }
                        : dbToken);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    Json(new { error = $"Error: {e.Message}. Stack Trace: {e.StackTrace}" }));
            }

            account.Password = string.Empty;
            account.PasswordSalt = string.Empty;

            return StatusCode((int)HttpStatusCode.OK, Json(new
            {
                account,
                token = savedToken
            }));
        }
    }
}