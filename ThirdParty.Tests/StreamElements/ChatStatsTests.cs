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
        private CouchDbStore<Account> _accountCollection;
        private Account _account;

        [TestInitialize]
        public void SetupTests()
        {
            var settingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _seClient = new StreamElementsService(settings);
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);

            _account = _accountCollection.GetAsync().GetAwaiter().GetResult().FirstOrDefault()?.Value;
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
                    Points = chatter.Amount
                };

                _account.Viewers.Add(viewer);
            });

            _accountCollection.AddOrUpdateAsync(_account).GetAwaiter().GetResult();
        }
    }
}
