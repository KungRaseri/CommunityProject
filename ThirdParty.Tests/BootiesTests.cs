using System;
using System.Linq;
using System.Net;
using Data.Helpers;
using Data.Models;
using NUnit.Framework;

namespace ThirdParty.Tests
{
    [TestFixture]
    public class BootiesTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private StreamElements _seClient;

        [SetUp]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.GetAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _seClient = new StreamElements(_settings);
        }

        [Test]
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

        [Test]
        public void GetAllTimeBooties_ReturnsGetTopBootiesResponse()
        {
            var expectedUsername = "blazdnconfuzd";
            var minimumPointAmount = 1000;
            var expectedTotal = 1;

            var bootiesResponse = _seClient.GetAllTimePoints().GetAwaiter().GetResult();

            var maxPoints = bootiesResponse.Users.Max(bu => bu.Points);
            var blazdnconfuzdPoints = bootiesResponse.Users.FirstOrDefault(bu => bu.Username.Contains(expectedUsername))?.Points;

            Assert.IsTrue(maxPoints == blazdnconfuzdPoints, $"The max points: {maxPoints} did not match the expected value: {blazdnconfuzdPoints} ");
            Assert.IsTrue(bootiesResponse.Users.FirstOrDefault(bu => bu.Username.Contains(expectedUsername))?.Points > minimumPointAmount, $"The user's points did not meet the minimum expected value: {minimumPointAmount} ");
            //Assert.IsTrue(bootiesResponse._total == expectedTotal, $"The Total: {bootiesResponse._total} did not match the expected value: {expectedTotal}");
        }

        [Test]
        public void GetUserRank_ReturnsUser()
        {
            var expectedUsername = "blazdnconfuzd";
            var expectedRank = 1;

            var user = _seClient.GetUserRank(expectedUsername).GetAwaiter().GetResult();

            Assert.IsTrue(user.Username == expectedUsername, $"Expected username: {expectedUsername}. Actual: {user.Username}");
            Assert.IsTrue(user.Rank == expectedRank, $"Expected rank: {expectedRank}. Actual: {user.Rank}");
        }

        [Test, Ignore("Untestable until they fix their shit and respond with status code in each response.")]
        public void GetUserRank_UserDoesNotExist_ReturnsErrorResponse()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var username = "asjhfjahggaw233awuea3q25q23ag3";

            var user = _seClient.GetUserRank(username).GetAwaiter().GetResult();

            Assert.IsTrue((HttpStatusCode)user.StatusCode == expectedStatusCode, $"Expected status code: {expectedStatusCode}. Actual: {user.StatusCode}");
            Assert.IsTrue(user.Error == "not found");
            Assert.IsTrue(user.Message == $"{username} was not found");
        }

        [Test]
        public void GetPointsByUser_ReturnsUser()
        {
            var expectedUsername = "blazdnconfuzd";
            var expectedRank = 1;

            var user = _seClient.GetPointsByUser(expectedUsername).GetAwaiter().GetResult();

            Assert.IsTrue(user.Username == expectedUsername, $"Expected username: {expectedUsername}. Actual: {user.Username}");
            Assert.IsTrue(user.Rank == expectedRank, $"Expected rank: {expectedRank}. Actual: {user.Rank}");
        }

        [Test]
        public void GetPointsByUser_UserDoesNotExist_ReturnsErrorResponse()
        {
            var expectedStatusCode = HttpStatusCode.NotFound;
            var username = "asjhfjahggaw233awuea3q25q23ag3";

            var user = _seClient.GetPointsByUser(username).GetAwaiter().GetResult();

            Assert.IsTrue((HttpStatusCode)user.StatusCode == expectedStatusCode, $"Expected status code: {expectedStatusCode}. Actual: {user.StatusCode}");
            Assert.IsTrue(user.Error.ToLower() == "not found", $"Expected error: not found. Actual: {user.Error.ToLower()}");
            Assert.IsTrue(user.Message.Contains(username), $"Expected message to contain: {username}. Actual: {user.Message}");
        }
    }
}
