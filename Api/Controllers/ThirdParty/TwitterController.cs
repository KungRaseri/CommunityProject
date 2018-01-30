using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers.ThirdParty
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/tweet")]
    public class TwitterController : BaseApiController
    {
        private readonly Twatter _twitterClient;

        public TwitterController(IConfiguration configuration) : base(configuration)
        {
            _twitterClient = new Twatter(Settings);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult> Get(string name)
        {
            var tweet = await _twitterClient.GetLatestTweetFromTimeline(name);

            return Json(tweet.FullText.Replace("\n", " "));
        }
    }
}
