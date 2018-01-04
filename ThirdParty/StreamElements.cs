using System;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.StreamElements.Booties;
using Data.Models.StreamElements.Giveaways;
using Data.Models.StreamElements.Logs;
using Data.Models.StreamElements.LoyaltySettings;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace ThirdParty
{
    public class StreamElements
    {
        protected static Settings _settings;
        public RestClient StreamElementsClient { get; set; }


        public StreamElements(Settings settings)
        {
            _settings = settings;

            StreamElementsClient = new RestClient(_settings.StreamElementsAPIUrl)
            {
                Authenticator = new JwtAuthenticator(_settings.Keys.StreamElements)
            };
        }

        #region Giveaway API

        public async Task<GetAllGiveawaysResponse> GetAllGiveaways()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"giveaways/{_settings.Keys.Twitch.ChannelId}" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var giveawayResponse = JsonConvert.DeserializeObject<GetAllGiveawaysResponse>(response.Content);

            return giveawayResponse;
        }

        public async Task<Giveaway[]> GetPastGiveaways()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"giveaways/{_settings.Keys.Twitch.ChannelId}/history" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var giveawayResponse = JsonConvert.DeserializeObject<Giveaway[]>(response.Content);

            return giveawayResponse;
        }

        public async Task<Giveaway> GetSingleGiveaway(string giveawayId)
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"giveaways/{_settings.Keys.Twitch.ChannelId}/{giveawayId}" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync<Giveaway>(request);
            var giveaway = JsonConvert.DeserializeObject<Giveaway>(response.Content);

            return giveaway;
        }

        #endregion Giveaways API

        #region Points API

        public async Task<GetTopBootiesResponse> GetTopPoints()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"points/{_settings.Keys.Twitch.ChannelId}/top" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var getTopBootiesResponse = JsonConvert.DeserializeObject<GetTopBootiesResponse>(response.Content);

            return getTopBootiesResponse;
        }

        public async Task<GetTopBootiesResponse> GetAllTimePoints()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"points/{_settings.Keys.Twitch.ChannelId}/alltime" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var getTopBootiesResponse = JsonConvert.DeserializeObject<GetTopBootiesResponse>(response.Content);

            return getTopBootiesResponse;
        }

        public async Task<UserRankResponse> GetUserRank(string username)
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"points/{_settings.Keys.Twitch.ChannelId}/{username}/rank" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var user = JsonConvert.DeserializeObject<UserRankResponse>(response.Content);

            return user;
        }
        public async Task<UserRankResponse> GetPointsByUser(string username)
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"points/{_settings.Keys.Twitch.ChannelId}/{username}" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync(request);
            var user = JsonConvert.DeserializeObject<UserRankResponse>(response.Content);

            return user;
        }

        #endregion

        #region Logs API

        public async Task<Log[]> GetLogs()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"logs/{_settings.Keys.Twitch.ChannelId}" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync<Log[]>(request);
            var logs = JsonConvert.DeserializeObject<Log[]>(response.Content);

            return logs;
        }

        #endregion

        #region Loyalty API

        public async Task<LoyaltySettings> GetLoyaltySettings()
        {
            var request = new RestRequest() { Method = Method.GET, Resource = $"loyalty/{_settings.Keys.Twitch.ChannelId}" };

            var response = await StreamElementsClient.ExecuteGetTaskAsync<LoyaltySettings>(request);
            var loyaltySettings = JsonConvert.DeserializeObject<LoyaltySettings>(response.Content);

            return loyaltySettings;
        }

        #endregion
    }
}
