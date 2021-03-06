﻿using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Tweetinvi;
using Tweetinvi.Models;

namespace ThirdParty
{
    public class Twatter
    {
        public Twatter(ApplicationSettings settings)
        {
            ITwitterCredentials twitterCredentials = new Tweetinvi.Models.TwitterCredentials(settings.Keys.Twitter.ConsumerKey, settings.Keys.Twitter.ConsumerSecret, settings.Keys.Twitter.AccessToken, settings.Keys.Twitter.AccessTokenSecret);
            Auth.ApplicationCredentials = twitterCredentials;
        }

        public async Task<ITweet> GetLatestTweetFromTimeline(string name)
        {
            var user = await UserAsync.GetUserFromScreenName(name);
            var timeline = Timeline.GetUserTimeline(user.Id);

            return timeline.FirstOrDefault();
        }
    }
}
