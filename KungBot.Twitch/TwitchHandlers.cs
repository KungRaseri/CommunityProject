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
        private static TwitchService _twitchService;
        private static List<Command> _commandSettings;

        private static CouchDbStore<Account> _accountCollection;

        public static void Init(TwitchClient client, TwitchPubSub pubsubClient, ApplicationSettings appSettings, Account account,
            List<Command> settings, CouchDbStore<Account> accountCollection)
        {
            _client = client;
            _account = account;
            _commandSettings = settings;
            _twitchService = new TwitchService(appSettings);

            _accountCollection = accountCollection;

            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnNewSubscriber += OnNewSubscriber;
            _client.OnReSubscriber += ClientOnOnReSubscriber;
            _client.OnLog += OnLog;
            _client.OnConnectionError += OnConnectionError;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.OnUserTimedout += OnUserTimedOut;
            _client.OnUserBanned += ClientOnUserBanned;

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

        public static async void ClientOnUserBanned(object sender, OnUserBannedArgs e)
        {
            var client = new RestClient(WebSocketSettings.LocalBotCommandRelayUrl);
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.UserBan.Username} has been banned. Reason: {e.UserBan.BanReason}");

            await client.ExecuteGetTaskAsync(request);
        }

        public static async void OnUserTimedOut(object sender, OnUserTimedoutArgs e)
        {
            var client = new RestClient(WebSocketSettings.LocalBotCommandRelayUrl);
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.UserTimeout.Username} timed out for {e.UserTimeout.TimeoutDuration} seconds. Reason: {e.UserTimeout.TimeoutReason} Kappa");

            await client.ExecuteGetTaskAsync(request);
        }

        public static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            CheckForCommand(sender, e);
        }

        public static async Task HandleViewerExperience(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsBroadcaster)
                return;

            var viewer = await HandleNewViewer(sender, e);

            viewer.IsSubscriber = e.ChatMessage.IsSubscriber;
            viewer.Points += _account.TwitchBotSettings.DefaultExperienceAmount;

            _account.Viewers.Add(viewer);

            await _accountCollection.AddOrUpdateAsync(_account);

        }

        public static void CheckForCommand(object sender, OnChatCommandReceivedArgs e)
        {
            var commandText = e.Command.CommandText;

            var commandSettings = _commandSettings.Find(c => c.Name == commandText);

            if (commandSettings == null) return;
            if (!commandSettings.IsActive) CommandUtility.GetCommandByKey("LogError")(_client, _twitchService, e.Command, commandSettings);

            CommandUtility.GetCommandByKey(commandSettings.Identifier)(_client, _twitchService, e.Command, commandSettings);
        }

        public static void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: \n{e.Error.Exception}\n{e.Error.Message}");
        }

        public static void OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Data}");
        }

        public static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            HandleNewSubscriber(sender, e, e.Subscriber.DisplayName).GetAwaiter().GetResult();

            _client.SendMessage(e.Channel,
                e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}!");
        }

        private static void ClientOnOnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            throw new NotImplementedException();
        }

        public static async Task HandleNewSubscriber(object sender, OnNewSubscriberArgs onNewSubscriberArgs, string viewerDisplayName)
        {
            var viewer = _account.Viewers.FirstOrDefault(u =>
                u.Username.ToLowerInvariant().Equals(viewerDisplayName.ToLowerInvariant()));

            if (viewer != null)
            {
                _account.Viewers.Remove(viewer);
                viewer.IsSubscriber = true;
                _account.Viewers.Add(viewer);
                return;
            }

            viewer = new Viewer()
            {
                Username = viewerDisplayName,
                IsSubscriber = true,
                Points = _account.TwitchBotSettings.DefaultExperienceAmount,
                SubscribedMonthCount = 1
            };

            _account.Viewers.Add(viewer);

            await _accountCollection.AddOrUpdateAsync(_account);
        }

        public static void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!");
        }

        public static void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleViewerExperience(sender, e).GetAwaiter().GetResult();

            //keyword matching
            HandleKeywordMatching(sender, e);
            //blacklist
            HandleBlacklistMatching(sender, e);
            //whitelist
            HandleWhitelistMatching(sender, e);
        }

        public static void HandleWhitelistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        public static void HandleBlacklistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        public static void HandleKeywordMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        public static async Task<Viewer> HandleNewViewer(object sender, EventArgs e)
        {
            var client = (TwitchClient)sender;
            var onMessage = (e is OnMessageReceivedArgs message) ? message : null;
            var onSub = (e is OnNewSubscriberArgs sub) ? sub : null;
            var username = (onMessage != null)
                ? onMessage.ChatMessage.Username
                : onSub?.Subscriber.DisplayName.ToLowerInvariant();
            var channel = (onMessage != null)
                ? onMessage.ChatMessage.Channel
                : onSub?.Channel;
            var isSub = (onSub != null) || onMessage.ChatMessage.IsSubscriber;
            var subMonthCount = onMessage?.ChatMessage.SubscribedMonthCount ?? 0;

            var viewer = _account.Viewers.AsParallel().FirstOrDefault(u => u.Username.Equals(username));

            if (viewer != null) return viewer;

            client.SendMessage(channel, $"kungraHYPERS {username}, welcome to the stream!");

            viewer = new Viewer()
            {
                Username = username,
                IsSubscriber = isSub,
                Points = _account.TwitchBotSettings.DefaultExperienceAmount,
                SubscribedMonthCount = subMonthCount
            };

            _account.Viewers.Add(viewer);

            await _accountCollection.AddOrUpdateAsync(_account);

            return viewer;

        }

        public static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Channel}");
        }
    }
}
