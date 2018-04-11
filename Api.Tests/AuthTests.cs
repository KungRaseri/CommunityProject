using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Api.Tests
{
    [TestClass, Ignore("We'll need to come back to this later, need to lern2code")]
    public class AuthTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void AuthController_Register_ReturnsUser()
        {
            var username = Faker.Internet.UserName();
            var salt = BCrypt.Net.BCrypt.GenerateSalt();

            var user = new User
            {
                Email = Faker.Internet.Email(username),
                TwitchUsername = username,
                Password = Crypto.PasswordCrypt(username, salt),
                PasswordSalt = salt
            };
        }

        [DataTestMethod]
        [DataRow("", "asjfhashfkajhsdf")]
        [DataRow("asdfjhakjsdfhkajshdf", "")]
        public void AuthController_Register_MissingParameters_ReturnsBadRequest(string email, string password)
        {

        }

        [TestMethod]
        public void AuthController_Register_AccountExists_ReturnsBadRequest()
        {

        }
    }
}
