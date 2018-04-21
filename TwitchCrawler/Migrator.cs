using System;
using System.Linq;
using Data;
using Data.Helpers;
using Data.Models;
using Data.Models.Twitch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThirdParty;
using Tweetinvi.Core.Extensions;
using TwitchLib.Api.Models.v5.Channels;

namespace TwitchCrawler
{
    [TestClass, TestCategory("Migrator")]
    public class Migrator
    {
        private CouchDbStore<Vod> _vodCollection;
        private TwitchService _twitchService;

        [TestInitialize]
        public void SetUpTests()
        {
            var settingsCollection = new CouchDbStore<Settings>(Settings.CouchDbUrl);
            var settings = settingsCollection.FindAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();
            _vodCollection = new CouchDbStore<Vod>(Settings.CouchDbUrl);
            _twitchService = new TwitchService(settings);
        }

        [TestMethod]
        public void MigrateTwitchVODMetadata()
        {
            var channelId = _twitchService.GetChannelIdFromChannelName("dasmehdi").GetAwaiter().GetResult();

            ChannelVideos videos = null;

            var offset = 0;

            do
            {
                videos = _twitchService.GetVideosFromChannelId(channelId, offset).GetAwaiter().GetResult();

                videos.Videos.ForEach(video =>
                {
                    var vod = new Vod()
                    {
                        ImportedAt = DateTime.UtcNow,
                        Video = video
                    };

                    var dbVod = _vodCollection.AddOrUpdateAsync(vod).GetAwaiter().GetResult();
                    Assert.IsInstanceOfType(dbVod, typeof(Vod));
                });

                offset += 100;
            } while (offset <= videos.Total);
        }

        [TestMethod]
        public void TestTheShitThatWasJustAdded()
        {
            var results = _vodCollection.GetAsync().GetAwaiter().GetResult();
            var expectedTitlePartial = "Lil Tuggz";

            var video = results.FirstOrDefault(v => v.Value.Video.Title.Contains(expectedTitlePartial));

            Assert.IsNotNull(video);
            Assert.IsTrue(video.Value.Video.Title.Contains(expectedTitlePartial));
        }
    }
}
