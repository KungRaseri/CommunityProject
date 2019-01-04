using System.Collections.Generic;
using System.Linq;
using System.Net;
using Data;
using Data.Helpers;
using Data.Models;
using FizzWare.NBuilder;
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

            //var username = "intoxikaite";
            //var email = Faker.Internet.Email(username);
            //var password = "$2b$10$RBcOZafEZ1qTC8qX1TrpPuEZP0E1iTzsTRGE1xux.mdojpguR4.92";
            //var passwordSalt = "$2b$10$RBcOZafEZ1qTC8qX1TrpPu";

            //var account = Builder<Account>
            //    .CreateNew()
            //    .With(a => a._id = null)
            //    .With(a => a._rev = null)
            //    .With(a => a.Username = username)
            //    .With(a => a.Email = email)
            //    .With(a => a.Password = password)
            //    .With(a => a.PasswordSalt = passwordSalt)
            //    //.With(a => a.Viewers = Builder<Viewer>.CreateListOfSize(0).Build().ToList())
            //    //.With(a => a.TwitchBotSettings = Builder<TwitchBotSettings>.CreateNew().Build())
            //    //.With(a => a.DiscordBotSettings = Builder<DiscordBotSettings>.CreateNew().Build())
            //    //.With(a => a.ViewerRanks = Builder<ViewerRank>.CreateListOfSize(15).Build().ToList())
            //    .Build();

            //_account = _accountCollection.AddOrUpdateAsync(account).GetAwaiter().GetResult();
        }

        [TestMethod, Ignore]
        public void GetChatStats_ReturnsChatStats()
        {
            var chatstats = _seClient.GetChatStats("intoxikaite").GetAwaiter().GetResult();
            _account.Viewers = new List<Viewer>();

            chatstats.Chatters.OrderByDescending(c => c.Amount).ForEach(chatter =>
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
