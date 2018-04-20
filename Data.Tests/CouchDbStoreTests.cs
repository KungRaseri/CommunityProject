using System;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class CouchDbStoreTests
    {
        private CouchDbStore<User> _usersCollection { get; set; }
        
        [TestInitialize]
        public void SetUpTests()
        {
            var settingsCollection = new CouchDbStore<Settings>(ApplicationConstants.CouchDbLocalUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();

            _usersCollection = new CouchDbStore<User>(settings.CouchDbUri);

            var user = new User()
            {
                Email = "buttlicker@slurrrrrp.com"
            };

            _usersCollection.AddOrUpdateAsync(user).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void FindUserByEmail_ReturnsUser()
        {
            var email = "buttlicker@slurrrrrp.com";

            var user = _usersCollection.FindUserByEmail(email).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(user, typeof(User));
            Assert.AreEqual(user.Email, email);
        }

        [TestMethod]
        public void FindUserByEmail_UserDoesNotExist_ReturnsNull()
        {
            var user = _usersCollection.FindUserByEmail(string.Empty).GetAwaiter().GetResult();

            Assert.IsNull(user);
        }
    }
}
