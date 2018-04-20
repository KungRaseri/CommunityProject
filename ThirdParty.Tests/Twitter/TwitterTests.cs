using Data;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Models;

namespace ThirdParty.Tests.Twitter
{
    [TestClass]
    public class TwitterTests
    {
        private TwitterService _twitterClient;

        [TestInitialize]
        public void SetupTests()
        {
            var settingsCollection = new CouchDbStore<Settings>(ApplicationConstants.CouchDbLocalUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _twitterClient = new TwitterService(settings);
        }

        [TestMethod]
        public void GetLatestTweet_ReturnsLatestTweet()
        {
            var tweet = _twitterClient.GetLatestTweetFromTimeline("dasmehdi").GetAwaiter().GetResult();

            Assert.IsInstanceOfType(tweet, typeof(ITweet));
        }
    }
}
