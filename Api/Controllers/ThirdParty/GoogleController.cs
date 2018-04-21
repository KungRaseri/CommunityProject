using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

namespace Api.Controllers.ThirdParty
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/google")]
    [ResponseCache(CacheProfileName = "Default")]
    public class GoogleController : BaseApiController
    {
        private readonly GoogleService _googleService;

        public GoogleController(IConfiguration configuration) : base(configuration)
        {
            _googleService = new GoogleService(_settings);
        }

        [HttpGet("youtube/{channelId}/latest")]
        [Produces("text/plain")]
        [ResponseCache(Duration = 43200, NoStore = false, Location = ResponseCacheLocation.Any,
            VaryByHeader = "Accept-Encoding")]
        public async Task<string> GetLatestYouTubeVideo(string channelId)
        {
            var video = await _googleService.Youtube.GetLatestVideo(channelId);

            if (video == null) return "Channel not found. Please try another.";

            var url = $"https://www.youtube.com/watch?v={video.Id.VideoId}";
            var shortUrl = await _googleService.UrlShortener.ShortenUrl(url);

            return $"{video.Snippet.Title} - {shortUrl}";
        }
    }
}