using System.Collections.Generic;
using System.Linq;
using Data.Helpers;
using Data.Models;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitchLib.Api.Models.ThirdParty.UsernameChange;

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
            _accountCollection.CreateView("account", "twitchbotsettings", "doc.twitchbotsettings").GetAwaiter().GetResult();
            _accountCollection.CreateView("account", "discordbotsettings", "doc.discordbotsettings").GetAwaiter().GetResult();
            _accountCollection.CreateView("account", "viewers", "doc.viewers").GetAwaiter().GetResult();
            _accountCollection.CreateView("account", "viewerranks", "doc.viewerranks").GetAwaiter().GetResult();
        }

        [TestMethod]
        public void VerifyDatabaseExists()
        {
            var response = _accountCollection.GetDatabase().GetAwaiter().GetResult();

            Assert.IsTrue(string.IsNullOrEmpty(response.Error));
            Assert.IsTrue(response.DbName.Equals("account"));
        }

        [TestMethod]
        public void AddDefaultAccount()
        {
            var username = Faker.Internet.UserName();
            var email = Faker.Internet.Email(username);
            var password = "$2b$10$RBcOZafEZ1qTC8qX1TrpPuEZP0E1iTzsTRGE1xux.mdojpguR4.92";
            var passwordSalt = "$2b$10$RBcOZafEZ1qTC8qX1TrpPu";

            var account = Builder<Account>
                .CreateNew()
                .With(a => a._id = null)
                .With(a => a._rev = null)
                .With(a => a.Username = username)
                .With(a => a.Email = email)
                .With(a => a.Password = password)
                .With(a => a.PasswordSalt = passwordSalt)
                .With(a => a.Viewers = Builder<Viewer>.CreateListOfSize(100).Build().ToList())
                .With(a => a.TwitchBotSettings = Builder<TwitchBotSettings>.CreateNew().Build())
                .With(a => a.DiscordBotSettings = Builder<DiscordBotSettings>.CreateNew().Build())
                .With(a => a.ViewerRanks = Builder<ViewerRank>.CreateListOfSize(15).Build().ToList())
                .Build();

            var response = _accountCollection.AddOrUpdateAsync(account).GetAwaiter().GetResult();

            Assert.IsFalse(string.IsNullOrEmpty(response._id));
        }
    }
}
