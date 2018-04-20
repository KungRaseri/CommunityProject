﻿using System.Linq;
using Data.Helpers;
using Data.Models;
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
        protected readonly Settings Settings;

        public BaseApiController(IConfiguration configuration)
        {
            Configuration = configuration;

            var configSettings = Configuration.GetSection("Settings");

            var settingCollection = new CouchDbStore<Settings>(configSettings.GetSection("CouchDbUri").Value);

            Settings = settingCollection.GetAsync().GetAwaiter().GetResult().FirstOrDefault()?.Value;

            TokenCollection = new CouchDbStore<Token>(Settings?.CouchDbUri);
            GoogleClient = new GoogleService(Settings);
        }

        protected CouchDbStore<Token> TokenCollection { get; set; }
    }
}