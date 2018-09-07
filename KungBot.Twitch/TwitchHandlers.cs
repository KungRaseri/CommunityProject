using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using KungBot.Twitch.Commands;
using RestSharp;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace KungBot.Twitch
{
    public class TwitchHandlers
    {
        private static TwitchClient _client;
        private static Account _account;
        private static CouchDbStore<Viewer> _viewerCollection;
        private static TwitchService _twitchService;
        private static List<Command> _commandSettings;

        public static void Init(ChannelManager channelManager, TwitchPubSub pubsubClient, ApplicationSettings appSettings, Account account,
            CouchDbStore<Viewer> viewerCollection, List<Command> settings)
        {
            _client = channelManager.GetClient();
            _account = account;
            _viewerCollection = viewerCollection;
            _commandSettings = settings;
            _twitchService = new TwitchService(appSettings);

            pubsubClient.OnPubSubServiceConnected += TwitchPubSubOnOnPubSubServiceConnected;
            pubsubClient.OnPubSubServiceClosed += TwitchPubSubOnOnPubSubServiceClosed;
            pubsubClient.OnChannelSubscription += TwitchPubSubOnOnChannelSubscription;
            pubsubClient.OnFollow += TwitchPubSubOnOnFollow;
            pubsubClient.OnEmoteOnly += TwitchPubSubOnOnEmoteOnly;
            pubsubClient.OnEmoteOnlyOff += TwitchPubSubOnOnEmoteOnlyOff;

            pubsubClient.ListenToFollows(appSettings?.Keys.Twitch.ChannelId);
            pubsubClient.ListenToSubscriptions(appSettings?.Keys.Twitch.ChannelId);
            pubsubClient.ListenToChatModeratorActions(_account?.TwitchBotSettings.Username, appSettings?.Keys.Twitch.ChannelId);
        }

        public static void TwitchPubSubOnOnEmoteOnly(object sender, OnEmoteOnlyArgs e)
        {
            _client.SendMessage("kungraseri", $"emote mode on, ran by {e.Moderator}!");
        }

        public static void TwitchPubSubOnOnEmoteOnlyOff(object sender, OnEmoteOnlyOffArgs e)
        {
            _client.SendMessage("kungraseri", $"emote mode off, ran by {e.Moderator}!");
        }

        public static void TwitchPubSubOnOnFollow(object sender, OnFollowArgs e)
        {
            _client.SendMessage("kungraseri", $"Thank you for the follow, {e.DisplayName}!");
        }

        public static void TwitchPubSubOnOnPubSubServiceClosed(object sender, EventArgs e)
        {
            Console.WriteLine("pubsub service closed");
        }

        public static void TwitchPubSubOnOnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
        {
            _client.SendMessage("kungraseri", $"{e.Subscription.DisplayName} just subscribed with a {e.Subscription.SubscriptionPlanName} sub!");
        }

        public static void TwitchPubSubOnOnPubSubServiceConnected(object sender, EventArgs e)
        {
            Console.WriteLine("pubsub service open");
        }

    }
}
