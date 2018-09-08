using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Seed
{
    [TestClass]
    public class TokenSeed
    {
        private CouchDbStore<Token> _tokenCollection { get; set; }

        [TestInitialize]
        public void CreateDatabaseAndViews()
        {
            _tokenCollection = new CouchDbStore<Token>(ApplicationSettings.CouchDbUrl);
            _tokenCollection.CreateDatabase().GetAwaiter().GetResult();
            _tokenCollection.CreateDesignDocument().GetAwaiter().GetResult();
            _tokenCollection.CreateView("token").GetAwaiter().GetResult();
            _tokenCollection.CreateView("token", "accountid", "doc.accountid").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void VerifyDatabaseExists()
        {
            var response = _tokenCollection.GetDatabase().GetAwaiter().GetResult();

            Assert.IsTrue(string.IsNullOrEmpty(response.Error));
            Assert.IsTrue(response.DbName.Equals("token"));
        }
    }
}
