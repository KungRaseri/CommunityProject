using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Extensions;
using Data.Helpers;
using Data.Models;
using Data.Models.Twitch;
using TwitchLib.Api;
using TwitchLib.Api.Models.v5.Channels;
using TwitchLib.Api.Models.v5.Videos;

namespace ThirdParty
{
    public class TwitchService
    {
        private readonly TwitchAPI _twitchApi;

        public TwitchService(Settings settings)
        {
            _twitchApi = new TwitchAPI();
            _twitchApi.Settings.SetClientIdAsync(settings.Keys.Twitch.ClientId).GetAwaiter().GetResult();
            _twitchApi.Settings.SetAccessTokenAsync(settings.Keys.Twitch.Bot.Oauth).GetAwaiter().GetResult();
        }

        public async Task<TimeSpan?> GetUpTimeByChannel(string channelId)
        {
            return await _twitchApi.Streams.v5.GetUptimeAsync(channelId);
        }

        public async Task<string> GetChannelIdFromChannelName(string channelName)
        {
            var channels = await _twitchApi.Search.v5.SearchChannelsAsync(channelName);
            var channel = channels.Channels.FirstOrDefault(ch => ch.Name.ToLower().Contains(channelName.ToLower()));

            return channel.Id;
        }

        public async Task<ChannelVideos> GetVideosFromChannelId(string channelId, int? offset = null, List<string> broadcastTypes = null)
        {
            var videos = await _twitchApi.Channels.v5.GetChannelVideosAsync(channelId, 100, offset, broadcastTypes);

            return videos;
        }

        public async Task<Video> GetVideoFromChannelIdWithTag(string channelId, string tag)
        {
            Video video = null;
            var offset = 0;

            while (video == null)
            {
                var videos = await GetVideosFromChannelId(channelId, offset);

                video = videos.Videos.FirstOrDefault(v => v.TagList.StripAndLower().Contains(tag.StripAndLower()));
                offset += 100;
                if (offset > videos.Total)
                    break;
            }

            return video;
        }

        public async Task<Video> GetVideoFromChannelIdWithGame(string channelId, string game)
        {
            Video video = null;
            var offset = 0;

            while (video == null)
            {
                var videos = await GetVideosFromChannelId(channelId, offset);

                video = videos.Videos.FirstOrDefault(v => string.Equals(v.Game.StripAndLower(), game.StripAndLower()));
                offset += 100;
                if (offset > videos.Total)
                    break;
            }

            return video;
        }

        public async Task<Video> GetVideoFromChannelIdWithTitle(string channelId, string title)
        {
            Video video = null;
            var offset = 0;

            while (video == null)
            {
                var videos = await GetVideosFromChannelId(channelId, offset);

                video = videos.Videos.FirstOrDefault(v => v.Title.StripAndLower().Contains(title.StripAndLower()));
                offset += 100;
                if (offset > videos.Total)
                    break;
            }

            return video;
        }
    }
}
