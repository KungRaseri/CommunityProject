using System.Linq;
using Data.Helpers;
using Data.Models;
using Data.Models.StreamElements.Logs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class LogsTests
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
        public void GetLogs_ReturnsLogsArray()
        {
            var logsResponse = _seClient.GetLogs().GetAwaiter().GetResult();

            Assert.IsTrue(logsResponse.Any(), "Expected more than one log entry");
            Assert.IsTrue(logsResponse.GetType() == typeof(Log[]));
        }
    }
}
