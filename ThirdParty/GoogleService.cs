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
        public YoutubeEndpoint Youtube { get; set; }

        public GoogleService(Settings settings)
        {
            Youtube = new YoutubeEndpoint(settings.Keys.GoogleApiKey);
        }

        public class YoutubeEndpoint
        {
            private readonly YouTubeService YoutubeService;

            public YoutubeEndpoint(string apiKey)
            {
                YoutubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = apiKey
                });

            }

            public async Task<SearchResult> GetLatestVideo(string channelId)
            {
                var searchRequest = YoutubeService.Search.List("snippet");
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
