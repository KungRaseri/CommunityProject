using System.Threading.Tasks;
using Humanizer;
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
        private readonly GoogleService _googleService;
        private readonly TwitterService _twitterClient;

        public TwitterController(IConfiguration configuration) : base(configuration)
        {
            _twitterClient = new TwitterService(_settings);
            _googleService = new GoogleService(_settings);
        }

        [HttpGet("{name}/latest")]
        [Produces("text/plain")]
        public async Task<string> Get(string name)
        {
            var tweet = await _twitterClient.GetLatestTweetFromTimeline(name);

            if (tweet == null) return "No tweets could be found.";

            var shortUrl = await _googleService.UrlShortener.ShortenUrl(tweet.Url);

            return $"{tweet.FullText.Replace("\n", " ")} - {shortUrl} - {tweet.CreatedAt.Humanize()}";
        }
    }
}