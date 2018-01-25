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
    public class Twitter
    {
        private readonly ITwitterCredentials TwitterCredentials;

        public Twitter(Settings settings)
        {
            TwitterCredentials = new Tweetinvi.Models.TwitterCredentials(settings.Keys.Twitter.ConsumerKey, settings.Keys.Twitter.ConsumerSecret, settings.Keys.Twitter.AccessToken, settings.Keys.Twitter.AccessTokenSecret);
            Auth.ApplicationCredentials = TwitterCredentials;
        }

        public async Task<ITweet> GetLatestTweet(string name)
        {
            var tweetParameters = new SearchTweetsParameters("dasmehdi")
            {
                TweetSearchType = TweetSearchType.OriginalTweetsOnly
            };

            var tweets = await SearchAsync.SearchTweets(tweetParameters);

            return tweets.LastOrDefault();
        }
    }
}
