using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Extensions;
using Data.Helpers;
using Data.Models;
using Data.Models.Twitch;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Api.V5.Models.Subscriptions;
using TwitchLib.Api.V5.Models.Videos;
using TwitchLib.PubSub;

namespace ThirdParty
{
    public class TwitchService
    {
        private readonly TwitchAPI _twitchApi;
        private readonly TwitchPubSub _twitchPubSub;
        public TwitchService(ApplicationSettings settings)
        {
            _twitchPubSub = new TwitchPubSub();
            _twitchApi = new TwitchAPI();
            _twitchApi.Settings.ClientId = settings.Keys.Twitch.ClientId;
        }

        public async Task<TimeSpan?> GetUpTimeByChannel(string channelId)
        {
            return await _twitchApi.V5.Streams.GetUptimeAsync(channelId);
        }

        public async Task<string> GetChannelIdFromChannelName(string channelName)
        {
            var channels = await _twitchApi.V5.Search.SearchChannelsAsync(channelName);
            var channel = channels.Channels.FirstOrDefault(ch => ch.Name.ToLower().Contains(channelName.ToLower()));

            return channel.Id;
        }

        public async Task<ChannelVideos> GetVideosFromChannelId(string channelId, int? offset = null, List<string> broadcastTypes = null)
        {
            var videos = await _twitchApi.V5.Channels.GetChannelVideosAsync(channelId, 100, offset, broadcastTypes);

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

        public async Task<ChannelSubscribers> GetChannelSubscribers(string channelId)
        {
            ChannelSubscribers subscribers;
            try
            {
                subscribers = await _twitchApi.V5.Channels.GetChannelSubscribersAsync(channelId, authToken: "0cie0p6cqx4mie6i6ueci1w7rhzcux");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return subscribers;
        }

        public async Task<List<Subscription>> GetChannelSubscribers2(string channelId)
        {
            List<Subscription> subscribers;
            try
            {
                subscribers = await _twitchApi.V5.Channels.GetAllSubscribersAsync(channelId, "0cie0p6cqx4mie6i6ueci1w7rhzcux");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return subscribers;
        }
    }
}
