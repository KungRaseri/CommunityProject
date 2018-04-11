using System;
using Data.Helpers;
using Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitchLib.Api.Models.v5.Videos;

namespace ThirdParty.Tests.Twitch
{
    [TestClass]
    public class TwitchServiceTests
    {
        private CouchDbStore<Settings> _settingsCollection;
        private Settings _settings;
        private TwitchService _twitchService;

        [TestInitialize]
        public void SetupTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _twitchService = new TwitchService(_settings);
        }

        [TestMethod]
        public void GetChannelIdFromChannel_ReturnsChannelId()
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            Assert.IsTrue(channelId == "31557869");
        }

        [TestMethod]
        public void GetVideoFromChannelIdWithTag_ReturnsVideo()
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            var video = _twitchService.GetVideoFromChannelIdWithTag(channelId, "subnautica").GetAwaiter().GetResult();

            Assert.IsInstanceOfType(video, typeof(Video));
        }
        [DataTestMethod]
        [DataRow("Grand Theft Auto")]
        [DataRow("Subnautica")]
        [DataRow("Hunt: Showdown")]
        public void GetVideoFromChannelIdWithGame_ReturnsVideo(string game)
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            var video = _twitchService.GetVideoFromChannelIdWithGame(channelId, game).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(video, typeof(Video));
            Assert.IsTrue(video.Game.Contains(game));
        }

        [DataTestMethod]
        [DataRow("Lil Tuggz")]
        [DataRow("Subnautica")]
        [DataRow("Nino Chavez")]
        public void GetVideoFromChannelIdWithTitle_ReturnsVideo(string title)
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            var video = _twitchService.GetVideoFromChannelIdWithTitle(channelId, title).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(video, typeof(Video));
        }

        [TestMethod]
        public void GetUpTimeFromChannel_ReturnsTimeSpan()
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            var uptime = _twitchService.GetUpTimeByChannel(channelId).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(uptime, typeof(TimeSpan?));
        }
    }
}
