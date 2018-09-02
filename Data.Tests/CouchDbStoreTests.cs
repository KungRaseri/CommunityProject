using System;
using System.Collections.Generic;
using System.Linq;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class CouchDbStoreTests
    {
        private CouchDbStore<Account> _usersCollection { get; set; }
        private CouchDbStore<ViewerRank> _viewerRankCollection { get; set; }

        [TestInitialize]
        public void SetUpTests()
        {
            _usersCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);
            _viewerRankCollection = new CouchDbStore<ViewerRank>(ApplicationSettings.CouchDbUrl);
        }

        [TestMethod, Ignore]
        public void FindUserByEmail_ReturnsUser()
        {
            var email = "buttlicker@slurrrrrp.com";

            var user = _usersCollection.FindUserByEmail(email).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(user, typeof(Account));
            Assert.AreEqual(user.Email, email);
        }

        [TestMethod, Ignore]
        public void FindUserByEmail_UserDoesNotExist_ReturnsNull()
        {
            var user = _usersCollection.FindUserByEmail(string.Empty).GetAwaiter().GetResult();

            Assert.IsNull(user);
        }

        [TestMethod, Ignore]
        public void CreateView_ReturnsDocumentHeaderResponse()
        {
            var database = _usersCollection.CreateDatabase().Result;

            var viewAll = _usersCollection.CreateView("user").Result;

            var viewEmail = _usersCollection.CreateView("user", "email", "doc.email").Result;
        }

        [TestMethod, Ignore]
        public void CreateViewerRanks_ReturnsListofViewerRanks()
        {
            var listOfRanks = new List<ViewerRank>()
            {
                new ViewerRank()
                {
                    RankName = "Peasant",
                    ExperienceRequired = 0
                },
                new ViewerRank()
                {
                    RankName = "Esquire",
                    ExperienceRequired = 51
                },
                new ViewerRank()
                {
                    RankName = "Knight",
                    ExperienceRequired = 151
                },
                new ViewerRank()
                {
                    RankName = "Baronet",
                    ExperienceRequired = 301
                },
                new ViewerRank()
                {
                    RankName = "Baron",
                    ExperienceRequired = 501
                },
                new ViewerRank()
                {
                    RankName = "Viscount",
                    ExperienceRequired = 751
                },
                new ViewerRank()
                {
                    RankName = "Earl",
                    ExperienceRequired = 1101
                },
                new ViewerRank()
                {
                    RankName = "Duke",
                    ExperienceRequired = 1501
                },
                new ViewerRank()
                {
                    RankName = "Prince",
                    ExperienceRequired = 2001
                }
            };
            var dbRanks = new List<ViewerRank>();
            listOfRanks.ForEach(rank =>
            {
                dbRanks.Add(_viewerRankCollection.AddOrUpdateAsync(rank).GetAwaiter().GetResult());
            });

            Assert.IsTrue(dbRanks.FirstOrDefault(rank => rank.ExperienceRequired > 750 && rank.ExperienceRequired < 1000).RankName.Equals("Viscount"));
        }
    }
}
