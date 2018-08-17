using System.Linq;
using Data;
using Data.Helpers;
using Data.Models;
using Data.Models.StreamElements.Logs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class LogsTests
    {
        private StreamElementsService _seClient;

        [TestInitialize]
        public void SetupTests()
        {
            var settingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _seClient = new StreamElementsService(settings);
        }

        [TestMethod]
        public void GetLogs_ReturnsLogsArray()
        {
            var logsResponse = _seClient.GetLogs().GetAwaiter().GetResult();

            Assert.IsTrue(logsResponse.Any(), "Expected more than one log entry");
            Assert.IsTrue(logsResponse.GetType() == typeof(Log[]));
        }
    }
}
