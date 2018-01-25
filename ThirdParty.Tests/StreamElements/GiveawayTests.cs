using System.Linq;
using Data.Helpers;
using Data.Models;
using Data.Models.StreamElements.Giveaways;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests
{
    [TestClass]
    public class GiveawayTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private StreamElements _seClient;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.GetAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _seClient = new StreamElements(_settings);
        }

        [TestMethod]
        public void GetAllGiveaways_ReturnsGetGiveawayResponse()
        {
            var expectedMaximumTickets = 10;
            var expectedDescription = "4 games up for grabs!";

            var giveawayResponse = _seClient.GetAllGiveaways().GetAwaiter().GetResult();

            Assert.IsTrue(giveawayResponse.Active.Description.Contains(expectedDescription), $"The Description: {giveawayResponse.Active.Description} did not match the expected value: {expectedDescription} ");
            Assert.IsTrue(giveawayResponse.Active.MaxTickets == expectedMaximumTickets, $"The Maximum Tickets: {giveawayResponse.Active.MaxTickets} did not match the expected value: {expectedMaximumTickets}");
        }

        [TestMethod]
        public void GetPastGiveaways_ReturnsGiveawayArray()
        {
            var giveawayResponse = _seClient.GetPastGiveaways().GetAwaiter().GetResult();

            Assert.IsTrue(giveawayResponse.Any(), "Expected more than zero giveaways");
            Assert.IsTrue(giveawayResponse[0].Winners.Any(), $"Expected more than 0 winners. Actual: {giveawayResponse[0].Winners.Count()}");
        }

        [TestMethod]
        public void GetSingleGiveaway_ReturnsGiveaway()
        {
            var giveaways = _seClient.GetAllGiveaways().GetAwaiter().GetResult();
            var giveawayResponse = _seClient.GetSingleGiveaway(giveaways.Active._id).GetAwaiter().GetResult();

            Assert.IsTrue(giveawayResponse.GetType() == typeof(Giveaway), $"Expected the Giveaway type. Actual: {giveawayResponse.GetType()}");
            Assert.IsTrue(giveawayResponse._id == giveaways.Active._id, $"Expected giveaway id: {giveaways.Active._id}. Actual: {giveawayResponse._id}");
        }
    }
}
