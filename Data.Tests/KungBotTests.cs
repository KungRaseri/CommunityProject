using System;
using System.Linq;
using Data.Enumerations;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class KungBotTests
    {
        private CouchDbStore<Viewer> _viewersCollection { get; set; }

        [TestInitialize]
        public void SetUpTests()
        {
            _viewersCollection = new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl);
        }

        [TestMethod, Ignore]
        public void CreateNewViewer_ReturnsViewer()
        {
            var viewer = new Viewer()
            {
                Experience = 0,
                IsBanned = false,
                IsFollower = true,
                IsSubscriber = false,
                SubscriptionLevel = SubscriptionLevel.None,
                Username = "SneakyFellow"
            };

            var viewerRow = _viewersCollection.GetAsync("viewer-username", viewer.Username).GetAwaiter().GetResult();
            var dbViewer = viewerRow.FirstOrDefault()?.Value;
            if (dbViewer == null)
            {
                var dbResult = _viewersCollection.AddOrUpdateAsync(viewer).GetAwaiter().GetResult();
                Assert.IsTrue(_viewersCollection.Store.ExistsAsync(dbResult._id).GetAwaiter().GetResult());
            }
            else
            {
                dbViewer.IsSubscriber = true;
                var dbResult = _viewersCollection.AddOrUpdateAsync(dbViewer).GetAwaiter().GetResult();
                Assert.IsFalse(dbViewer._rev.Equals(dbResult._rev), "revision was the same");
            }
        }
    }
}
