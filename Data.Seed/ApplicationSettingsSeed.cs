using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Seed
{
    [TestClass]
    public class ApplicationSettingsSeed
    {
        private CouchDbStore<ApplicationSettings> _applicationSettingsCollection { get; set; }

        [TestInitialize]
        public void CreateDatabaseAndViews()
        {
            _applicationSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            _applicationSettingsCollection.CreateDatabase().GetAwaiter().GetResult();
            _applicationSettingsCollection.CreateDesignDocument().GetAwaiter().GetResult();
            _applicationSettingsCollection.CreateView("applicationsettings").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void VerifyDatabaseExists()
        {
            var response = _applicationSettingsCollection.GetDatabase().GetAwaiter().GetResult();

            Assert.IsTrue(string.IsNullOrEmpty(response.Error));
            Assert.IsTrue(response.DbName.Equals("applicationsettings"));
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
            var result = _applicationSettingsCollection.AddOrUpdateAsync(appSettings).GetAwaiter().GetResult();

            Assert.IsTrue(result.CookieToken.Equals(appSettings.CookieToken));
        }
    }
}
