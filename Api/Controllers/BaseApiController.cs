using System.Linq;
using Data.Helpers;
using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Produces("application/json")]
    public class BaseApiController : Controller
    {
        protected readonly IConfiguration Configuration;
        protected readonly GoogleService GoogleClient;
        protected readonly ApplicationSettings _settings;
        protected CouchDbStore<Token> TokenCollection { get; set; }

        public BaseApiController(IConfiguration configuration)
        {
            Configuration = configuration;

            var configSettings = Configuration.GetSection("Settings");

            var settingCollection = new CouchDbStore<ApplicationSettings>(configSettings.GetSection("CouchDbUri").Value);
            TokenCollection = new CouchDbStore<Token>(configSettings.GetSection("CouchDbUri").Value);

            _settings = settingCollection.GetAsync().GetAwaiter().GetResult().FirstOrDefault()?.Value;

            GoogleClient = new GoogleService(_settings);
        }
    }
}