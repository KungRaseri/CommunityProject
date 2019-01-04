using System.Collections.Generic;
using System.Linq;
using System.Net;
using Data;
using Data.Helpers;
using Data.Models;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi.Core.Extensions;

namespace ThirdParty.Tests.StreamElements
{
    [TestClass]
    public class PointsTests
    {
        private StreamElementsService _seClient;
        private CouchDbStore<Account> _accountCollection;

        [TestInitialize]
        public void SetupTests()
        {
            //TODO: Put default couchdburl in appsettings and transform during CI/CD
            var settingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var settings = settingsCollection.FindAsync("44679c0430e35c0aa717dedbd3016ca4").GetAwaiter().GetResult();
            var _twitchClient = new TwitchService(settings);
            var channelId = _twitchClient.GetChannelIdFromChannelName("intoxikaite").GetAwaiter().GetResult();
            settings.Keys.Twitch.ChannelId = channelId;
            _seClient = new StreamElementsService(settings);
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);
        }

        [TestMethod]
        public void GetTopBooties_ReturnsGetTopBootiesResponse()
        {
            var expectedUsername = "majxr";
            var minimumPointAmount = 1000;
            var minimumTotal = 10;

            var bootiesResponse = _seClient.GetTopPoints().GetAwaiter().GetResult();

            Assert.IsTrue(bootiesResponse.Users.Any(bu => bu.Username.Contains(expectedUsername)), $"The list of usernames did not contain the expected value: {expectedUsername} ");
            Assert.IsTrue(bootiesResponse.Users.FirstOrDefault(bu => bu.Username.Contains(expectedUsername))?.Points > minimumPointAmount, $"The user's points did not meet the minimum expected value: {minimumPointAmount} ");
            Assert.IsTrue(bootiesResponse._total > minimumTotal, $"The Total: {bootiesResponse._total} did not meet the minimum expected value: {minimumTotal}");
        }

        [TestMethod]
        public void GetAllTimeBooties_ReturnsGetTopBootiesResponse()
        {
            var expectedUsername = "blazdnconfuzd";
            var minimumPointAmount = 1000;

            var bootiesResponse = _seClient.GetAllTimePoints().GetAwaiter().GetResult();

            var maxPoints = bootiesResponse.Users.Max(bu => bu.Points);
            var blazdnconfuzdPoints = bootiesResponse.Users.FirstOrDefault(bu => bu.Username.Contains(expectedUsername))?.Points;

            Assert.IsTrue(maxPoints == blazdnconfuzdPoints, $"The max points: {maxPoints} did not match the expected value: {blazdnconfuzdPoints} ");
            Assert.IsTrue(bootiesResponse.Users.FirstOrDefault(bu => bu.Username.Contains(expectedUsername))?.Points > minimumPointAmount, $"The user's points did not meet the minimum expected value: {minimumPointAmount} ");
        }

        [TestMethod]
        public void GetUserRank_ReturnsUser()
        {
            var expectedUsername = "blazdnconfuzd";
            var expectedRank = 1;

            var user = _seClient.GetUserRank(expectedUsername).GetAwaiter().GetResult();

            Assert.IsTrue(user.Username == expectedUsername, $"Expected username: {expectedUsername}. Actual: {user.Username}");
            Assert.IsTrue(user.Rank == expectedRank, $"Expected rank: {expectedRank}. Actual: {user.Rank}");
        }

        [TestMethod, Ignore("Untestable until they fix their shit and respond with status code in each response.")]
        public void GetUserRank_UserDoesNotExist_ReturnsErrorResponse()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var username = "asjhfjahggaw233awuea3q25q23ag3";

            var user = _seClient.GetUserRank(username).GetAwaiter().GetResult();

            Assert.IsTrue((HttpStatusCode)user.StatusCode == expectedStatusCode, $"Expected status code: {expectedStatusCode}. Actual: {user.StatusCode}");
            Assert.IsTrue(user.Error == "not found");
            Assert.IsTrue(user.Message == $"{username} was not found");
        }

        [TestMethod]
        public void GetPointsByUser_ReturnsUser()
        {
            var expectedUsername = "blazdnconfuzd";
            var expectedRank = 1;

            var user = _seClient.GetPointsByUser(expectedUsername).GetAwaiter().GetResult();

            Assert.IsTrue(user.Username == expectedUsername, $"Expected username: {expectedUsername}. Actual: {user.Username}");
            Assert.IsTrue(user.Rank == expectedRank, $"Expected rank: {expectedRank}. Actual: {user.Rank}");
        }

        [TestMethod]
        public void GetPointsByUser_UserDoesNotExist_ReturnsErrorResponse()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var username = "asjhfjahggaw233awuea3q25q23ag3";

            var user = _seClient.GetPointsByUser(username).GetAwaiter().GetResult();

            Assert.IsTrue((HttpStatusCode)user.StatusCode == expectedStatusCode, $"Expected status code: {expectedStatusCode}. Actual: {user.StatusCode}");
            Assert.IsTrue(user.Error.ToLower() == "not found", $"Expected error: not found. Actual: {user.Error.ToLower()}");
            Assert.IsTrue(user.Message.Contains(username), $"Expected message to contain: {username}. Actual: {user.Message}");
        }

        [TestMethod]
        public void SeedAccount_PointsAndAlltimePoints()
        {
            var username = "intoxikaite";
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
                .With(a => new List<Viewer>())
                //.With(a => a.TwitchBotSettings = Builder<TwitchBotSettings>.CreateNew().Build())
                //.With(a => a.DiscordBotSettings = Builder<DiscordBotSettings>.CreateNew().Build())
                //.With(a => a.ViewerRanks = Builder<ViewerRank>.CreateListOfSize(15).Build().ToList())
                .Build();

            var _account = _accountCollection.AddOrUpdateAsync(account).GetAwaiter().GetResult();

            var chatstats = _seClient.GetAllTimePoints().GetAwaiter().GetResult();

            chatstats.Users.OrderByDescending(u => u.PointsAllTime).ForEach(user =>
            {
                var viewer = new Viewer()
                {
                    Username = user.Username,
                    Points = user.Points,
                    PointsAllTime = user.PointsAllTime
                };

                _account.Viewers.Add(viewer);
            });

            _accountCollection.AddOrUpdateAsync(_account).GetAwaiter().GetResult();
        }
    }
}
