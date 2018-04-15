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
                return "Channel not found. Please try another.";
            }

            var url = $"https://www.youtube.com/watch?v={video.Id.VideoId}";
            var shortUrl = url;

            return $"{video.Snippet.Title} - {shortUrl}";
        }
    }
}
