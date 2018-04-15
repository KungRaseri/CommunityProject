using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Google.Apis.Services;
using Google.Apis.Urlshortener.v1;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace ThirdParty
{
    public class GoogleService
    {
        protected static Settings _settings;

        public GoogleService(Settings settings)
        {
            _settings = settings;
        }

        public static class Youtube
        {
            public static async Task<SearchResult> GetLatestYoutubeVideo(string channelId)
            {
                var yt = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _settings.Keys.GoogleApiKey
                });

                var searchRequest = yt.Search.List("snippet");
                searchRequest.ChannelId = channelId;
                searchRequest.PublishedAfter = DateTime.Parse("01/01/2018");
                searchRequest.MaxResults = 10;
                searchRequest.Order = SearchResource.ListRequest.OrderEnum.Date;
                var results = await searchRequest.ExecuteAsync();
                var result = results.Items.FirstOrDefault();

                return result;
            }
        }
    }
}
