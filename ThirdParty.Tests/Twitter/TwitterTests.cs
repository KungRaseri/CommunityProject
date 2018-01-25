using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Models;

namespace ThirdParty.Tests.Twitter
{
    [TestClass]
    public class TwitterTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private ThirdParty.Twitter _twitterClient;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.GetAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _twitterClient = new ThirdParty.Twitter(_settings);
        }

        [TestMethod]
        public void GetLatestTweet_ReturnsLatestTweet()
        {
            var tweet = _twitterClient.GetLatestTweet("dasmehdi").GetAwaiter().GetResult();

            Assert.IsInstanceOfType(tweet, typeof(ITweet));
        }
    }
}
