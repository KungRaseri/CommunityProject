using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Seed.Mongo
{
    [TestClass]
    public class ApplicationSettingsMongoSeed
    {
        private MongoDbStore<ApplicationSettings> _applicationSettingsCollection { get; set; }

        [TestInitialize]
        public void CreateDatabaseAndViews()
        {
            _applicationSettingsCollection = new MongoDbStore<ApplicationSettings>(ApplicationSettings.MongoDbUrl);
        }

        [TestMethod]
        public void VerifyDatabaseExists()
        {
            var response = _applicationSettingsCollection.GetDatabase();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.DatabaseNamespace.DatabaseName.Equals("applicationsettings"));
        }

        [TestMethod]
        public void Create_ApplicationSettingsDocument()
        {
            var appSettings = new ApplicationSettings()
            {
                Keys = new Keys()
                {
                    Twitch = new TwitchCredentials()
                    {
                        ClientId = "[insert clientid here]",
                        ClientSecret = "[insert clientsecret here]",
                        ChannelId = "[insert channelid here]",
                        Bot = new TwitchCredentials.BotCredentials()
                        {
                            Username = "[insert bot username here]",
                            ClientId = "[insert bot clientid here]",
                            ClientSecret = "[insert bot clientsecret here]",
                            Oauth = "[insert bot oauth here]"
                        }
                    },
                    Discord = "[insert dicord token here]",
                    JWTSecurityKey = "[insert random string here, should be long]",
                    StreamElements = "[insert streamelements api key here]",
                    GoogleApiKey = "[insert google api key here]",
                    Twitter = new TwitterCredentials()
                    {
                        AccessToken = "[insert twitter accesstoken here]",
                        AccessTokenSecret = "[insert twitter accesstokensecret here]",
                        ConsumerKey = "[insert twitter consumerkey here]",
                        ConsumerSecret = "[insert twitter consumersecret here]"
                    }
                },
                CookieToken = "[insert random string here, must be 30 characters long]"
            };
            var result = _applicationSettingsCollection.Save(appSettings).GetAwaiter().GetResult();

            Assert.IsTrue(result.CookieToken.Equals(appSettings.CookieToken));
        }
    }
}
