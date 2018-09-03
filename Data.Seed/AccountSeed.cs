using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Seed
{
    [TestClass]
    public class AccountSeed
    {
        private CouchDbStore<Account> _accountCollection { get; set; }

        [TestInitialize]
        public void CreateDatabaseAndViews()
        {
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);
            _accountCollection.CreateDatabase().GetAwaiter().GetResult();
            _accountCollection.CreateDesignDocument().GetAwaiter().GetResult();
            _accountCollection.CreateView("account").GetAwaiter().GetResult();
            _accountCollection.CreateView("account", "email", "doc.email").GetAwaiter().GetResult();
            _accountCollection.CreateView("account", "username", "doc.username").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void VerifyDatabaseExists()
        {
            var response = _accountCollection.GetDatabase().GetAwaiter().GetResult();

            Assert.IsTrue(string.IsNullOrEmpty(response.Error));
            Assert.IsTrue(response.DbName.Equals("account"));
        }

    }
}
