using System.Linq;
using System.Net;
using Data;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Core.Extensions;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class ChatStatsTests
    {
        private StreamElementsService _seClient;
        private CouchDbStore<Viewer> _viewerCollection;

        [TestInitialize]
        public void SetupTests()
        {
            var settingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _seClient = new StreamElementsService(settings);
            _viewerCollection = new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl);
        }

        [TestMethod]
        public void GetChatStats_ReturnsChatStats()
        {
            var chatstats = _seClient.GetChatStats("KungRaseri").GetAwaiter().GetResult();

            chatstats.Chatters.ForEach(chatter =>
            {
                var viewer = new Viewer()
                {
                    Username = chatter.Name,
                    Experience = chatter.Amount
                };

                _viewerCollection.AddOrUpdateAsync(viewer).GetAwaiter().GetResult();
            });
        }
    }
}
