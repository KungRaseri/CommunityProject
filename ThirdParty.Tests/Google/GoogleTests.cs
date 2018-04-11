using Data.Helpers;
using Data.Models;
using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.Google
{
    [TestClass]
    public class GoogleTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private GoogleService _googleClient;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _googleClient = new GoogleService(_settings);
        }

        [TestMethod]
        public void GetLatestYoutubeVideo_ReturnsSearchListResponse()
        {
            var channelId = "UCH3dHHNk1pSgDFtGy_XdL0Q";
            var video = GoogleService.Youtube.GetLatestYoutubeVideo(channelId).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(video, typeof(SearchResult));
            Assert.IsTrue(video.Snippet.ChannelId == channelId);
        }

        [TestMethod]
        public void UrlShortenerService_ReturnsShortenedUrl()
        {
            var url = "https://www.youtube.com/watch?v=_8wDuGi4yMc";

            var shortUrl = GoogleService.UrlShortener.ShortenUrl(url).GetAwaiter().GetResult();

            Assert.IsTrue(true);
        }
    }
}
