using Data;
using Data.Helpers;
using Data.Models;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.Google
{
    [TestClass]
    public class GoogleTests
    {
        private GoogleService _googleService;

        [TestInitialize]
        public void SetupTests()
        {
            //TODO: Put default couchdburl in appsettings and transform during CI/CD
            var settingsCollection = new CouchDbStore<Settings>(Settings.CouchDbUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _googleService = new GoogleService(settings);
        }

        [TestMethod]
        public void GetLatestYoutubeVideo_ReturnsSearchListResponse()
        {
            var channelId = "UCH3dHHNk1pSgDFtGy_XdL0Q";
            var video = _googleService.Youtube.GetLatestVideo(channelId).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(video, typeof(SearchResult));
            Assert.IsTrue(video.Snippet.ChannelId == channelId);
        }

        [TestMethod]
        public void UrlShortenerService_ReturnsShortenedUrl()
        {
            var url = "https://www.youtube.com/watch?v=_8wDuGi4yMc";

            var shortUrl = _googleService.UrlShortener.ShortenUrl(url).Result;

            Assert.IsTrue(!string.IsNullOrEmpty(shortUrl));
        }
    }
}
