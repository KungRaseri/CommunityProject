using Data;
using Data.Helpers;
using Data.Models;
using Data.Models.StreamElements.LoyaltySettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class LoyaltyTests
    {
        private StreamElementsService _seClient;

        [TestInitialize]
        public void SetupTests()
        {
            var settingsCollection = new CouchDbStore<Settings>(ApplicationConstants.CouchDbLocalUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _seClient = new StreamElementsService(settings);
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
