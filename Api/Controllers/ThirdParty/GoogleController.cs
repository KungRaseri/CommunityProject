using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;

namespace Api.Controllers.ThirdParty
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/google")]
    public class GoogleController : BaseApiController
    {
        public GoogleController(IConfiguration configuration) : base(configuration)
        {
        }

        [HttpGet("youtube/{channelId}/latest")]
        [Produces("text/plain")]
        public async Task<string> GetLatestYouTubeVideo(string channelId)
        {
            var video = await GoogleService.Youtube.GetLatestYoutubeVideo(channelId);

            if (video == null)
            {
                return "No video with that title could be found. Please try another.";
            }

            var url = $"https://www.youtube.com/watch?v={video.Id.VideoId}";

            return $"{video.Snippet.Title} - {await GoogleService.UrlShortener.ShortenUrl(url)}";
        }
    }
}
