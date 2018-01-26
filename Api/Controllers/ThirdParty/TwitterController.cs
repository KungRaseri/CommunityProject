using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers.ThirdParty
{
    [Route("api/v{version:ApiVersion}/tweet")]
    public class TwitterController : BaseApiController
    {
        private readonly Tweeter _twitterClient;

        public TwitterController(IConfiguration configuration) : base(configuration)
        {
            _twitterClient = new Tweeter(Settings);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult> Get(string name)
        {
            var tweet = await _twitterClient.GetLatestTweetFromTimeline(name);

            return StatusCode((int)HttpStatusCode.OK, Json(tweet.FullText));
        }
    }
}
