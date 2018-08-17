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
        public UrlShortenerEndpoint UrlShortener { get; set; }

        public GoogleService(ApplicationSettings settings)
        {
            Youtube = new YoutubeEndpoint(settings.Keys.GoogleApiKey);
            UrlShortener = new UrlShortenerEndpoint(settings.Keys.GoogleApiKey);
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

        public class UrlShortenerEndpoint
        {
            private readonly UrlshortenerService _urlShortener;

            public UrlShortenerEndpoint(string apiKey)
            {
                _urlShortener = new UrlshortenerService(new BaseClientService.Initializer() { ApiKey = apiKey });
            }

            public async Task<string> ShortenUrl(string longUrl)
            {
                var url = new Google.Apis.Urlshortener.v1.Data.Url { LongUrl = longUrl };

                var shortUrl = await _urlShortener.Url.Insert(url).ExecuteAsync();

                return shortUrl.Id;
            }
        }
    }
}
