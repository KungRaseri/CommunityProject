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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers.Account
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly CouchDbStore<User> _userCollection;
        private readonly Crypto _cryptoService;

        public AuthController(IConfiguration configuration) : base(configuration)
        {
            _userCollection = new CouchDbStore<User>(Settings.CouchDbUrl);
            _cryptoService = new Crypto(_settings.CookieToken);
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                return StatusCode((int) HttpStatusCode.BadRequest,
                    Json(new {error = "Email or Password are required"}));

            var dbUser = await _userCollection.FindUserByEmail(user.Email);

            if (dbUser != null)
                return StatusCode((int) HttpStatusCode.BadRequest, Json(new {error = "Account already exists"}));

            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            var hashedPassword = _cryptoService.PasswordCrypt(user.Password, salt);

            user = new User
            {
                Email = user.Email,
                Password = hashedPassword,
                PasswordSalt = salt
            };

            dbUser = await _userCollection.AddOrUpdateAsync(user);

            dbUser.Password = string.Empty;
            dbUser.PasswordSalt = string.Empty;

            return StatusCode((int) HttpStatusCode.OK, Json(dbUser));
        }

        [Route("token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Token(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return StatusCode((int) HttpStatusCode.BadRequest,
                    Json(new {error = "Email or Password are required"}));

            var user = await _userCollection.FindUserByEmail(email);

            if (user == null)
                return StatusCode((int) HttpStatusCode.Unauthorized, Json(new {error = "Invalid credentials"}));

            var hashedPassword = _cryptoService.PasswordCrypt(password, user.PasswordSalt);

            if (!hashedPassword.Equals(user.Password))
                return StatusCode((int) HttpStatusCode.Unauthorized, Json(new {error = "Invalid credentials"}));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.PrimarySid, user._id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Keys.JWTSecurityKey));

            var jwt = new JwtSecurityToken(new JwtHeader(
                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)),
                new JwtPayload("kungraseri-api", "kungraseri-audience", claims, null, DateTime.UtcNow.AddDays(1)));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            Token savedToken;

            try
            {
                var dbToken = await TokenCollection.FindTokenByUserId(user._id);
                savedToken = await TokenCollection.AddOrUpdateTokenAsync(
                    dbToken == null || dbToken.Expiration < DateTime.UtcNow
                        ? new Token
                        {
                            Value = token,
                            UserId = user._id,
                            Issued = DateTime.UtcNow,
                            Expiration = jwt.ValidTo
                        }
                        : dbToken);
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError,
                    Json(new {error = $"Error: {e.Message}. Stack Trace: {e.StackTrace}"}));
            }

            user.Password = string.Empty;
            user.PasswordSalt = string.Empty;

            return StatusCode((int) HttpStatusCode.OK, Json(new
            {
                user,
                token = savedToken
            }));
        }
    }
}