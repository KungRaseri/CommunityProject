using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Extensions;
using Data.Helpers;
using Data.Models.Twitch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ThirdParty;
using TwitchLib.Api.V5.Models.Videos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers.ThirdParty
{
    [ApiVersion("1")]
    [Route("api/v{version:ApiVersion}/twitch")]
    public class TwitchController : BaseApiController
    {
        private readonly GoogleService _googleService;
        private readonly TwitchService _twitchClient;
        private readonly CouchDbStore<Vod> _vodCollection;

        public TwitchController(IConfiguration configuration) : base(configuration)
        {
            _twitchClient = new TwitchService(_settings);
            _vodCollection = new CouchDbStore<Vod>(Data.Models.ApplicationSettings.CouchDbUrl);
            _googleService = new GoogleService(_settings);
        }

        [HttpGet("{channelName}/vod/game/{game}")]
        [Produces("text/plain")]
        public async Task<string> GetVodByGame(string channelName, string game)
        {
            Video video;

            var dbVideos = (await _vodCollection.GetAsync()).ToList();

            if (!dbVideos.All(v => v.Value.Video.Channel.Name.Contains(channelName)))
            {
                var channelId = await _twitchClient.GetChannelIdFromChannelName(channelName);

                video = await _twitchClient.GetVideoFromChannelIdWithGame(channelId, game);
            }
            else
            {
                try
                {
                    var videoRow = dbVideos.Where(dbv => dbv.Value.Video.Game.Contains(game)).Select(v => v.Value)
                        .FirstOrDefault();
                    video = videoRow?.Video;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (video == null) return "No video for that game could be found. Please try another.";

            var shortUrl = _googleService.UrlShortener.ShortenUrl($"https://www.twitch.tv/videos/{video.Id}");

            return $"{video.Title} - {shortUrl}";
        }

        [HttpGet("{channelName}/vod/title/{title}")]
        [Produces("text/plain")]
        public async Task<string> GetVodByTitle(string channelName, string title)
        {
            Video video;

            var dbVideo = await _vodCollection.GetAsync();

            if (dbVideo == null)
            {
                var channelId = await _twitchClient.GetChannelIdFromChannelName(channelName);

                video = await _twitchClient.GetVideoFromChannelIdWithTitle(channelId, title);
            }
            else
            {
                video = dbVideo.FirstOrDefault(v => v.Value.Video.Title.StripAndLower().Contains(title.StripAndLower()))
                    ?.Value.Video;
            }

            if (video == null) return "No video for that game could be found. Please try another.";

            var shortUrl = _googleService.UrlShortener.ShortenUrl($"https://www.twitch.tv/videos/{video.Id}");

            return $"{video.Title} - {shortUrl}";
        }
    }
}