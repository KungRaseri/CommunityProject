using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace ThirdParty
{
    public class Tweeter
    {
        private readonly ITwitterCredentials TwitterCredentials;

        public Tweeter(Settings settings)
        {
            TwitterCredentials = new Tweetinvi.Models.TwitterCredentials(settings.Keys.Twitter.ConsumerKey, settings.Keys.Twitter.ConsumerSecret, settings.Keys.Twitter.AccessToken, settings.Keys.Twitter.AccessTokenSecret);
            Auth.ApplicationCredentials = TwitterCredentials;
        }

        public async Task<ITweet> GetLatestTweetFromTimeline(string name)
        {
            var user = await UserAsync.GetUserFromScreenName(name);
            var timeline = Timeline.GetUserTimeline(user.Id);

            return timeline.FirstOrDefault();
        }
    }
}
