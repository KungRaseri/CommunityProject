using Data.Helpers;
using Data.Models;
using Data.Models.StreamElements.LoyaltySettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class LoyaltyTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private StreamElementsService _seClient;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _seClient = new StreamElementsService(_settings);
        }

        [TestMethod]
        public void GetLoyaltySettings_ReturnsLoyaltySettings()
        {
            var loyaltyResponse = _seClient.GetLoyaltySettings().GetAwaiter().GetResult();

            Assert.IsTrue(loyaltyResponse.Loyalty.Enabled, $"Expected loyalty system to be enabled. Actual: {loyaltyResponse.Loyalty.Enabled}");
            Assert.IsTrue(loyaltyResponse.Loyalty.GetType() == typeof(Loyalty), $"Expected type: {typeof(Loyalty)}. Actual: {loyaltyResponse.Loyalty.GetType()}");
        }
    }
}
