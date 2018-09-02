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
        private static ApplicationSettings _appSettings;
        private static CouchDbStore<Viewer> _viewerCollection;
        private static TwitchService _twitchService;
        private static List<Command> _commands;

        public static void Init(TwitchClient client, TwitchPubSub pubsubClient, ApplicationSettings appSettings, Account account,
            CouchDbStore<Viewer> viewerCollection, List<Command> commands)
        {
            _client = client;
            var _twitchPubSub = pubsubClient;
            _account = account;
            _appSettings = appSettings;
            _viewerCollection = viewerCollection;
            _commands = commands;
            _twitchService = new TwitchService(_appSettings);

            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnNewSubscriber += OnNewSubscriber;
            _client.OnLog += OnLog;
            _client.OnConnectionError += OnConnectionError;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.OnUserTimedout += OnUserTimedOut;
            _client.OnUserBanned += ClientOnUserBanned;
            _twitchPubSub.OnPubSubServiceConnected += TwitchPubSubOnOnPubSubServiceConnected;
            _twitchPubSub.OnPubSubServiceClosed += TwitchPubSubOnOnPubSubServiceClosed;
            _twitchPubSub.OnChannelSubscription += TwitchPubSubOnOnChannelSubscription;
            _twitchPubSub.OnFollow += TwitchPubSubOnOnFollow;
            _twitchPubSub.OnEmoteOnly += TwitchPubSubOnOnEmoteOnly;
            _twitchPubSub.OnEmoteOnlyOff += TwitchPubSubOnOnEmoteOnlyOff;

            _twitchPubSub.ListenToFollows(_appSettings?.Keys.Twitch.ChannelId);
            _twitchPubSub.ListenToSubscriptions(_appSettings?.Keys.Twitch.ChannelId);
            _twitchPubSub.ListenToChatModeratorActions(_account?.TwitchBotSettings.Username, _appSettings?.Keys.Twitch.ChannelId);
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

            var dbViewer = await HandleNewViewer(sender, e);

            dbViewer.IsSubscriber = e.ChatMessage.IsSubscriber;
            dbViewer.Points += _account.TwitchBotSettings.DefaultExperienceAmount;

            await _viewerCollection.AddOrUpdateAsync(dbViewer);
        }

        public static void CheckForCommand(object sender, OnChatCommandReceivedArgs e)
        {
            var commandText = e.Command.CommandText;

            var runMe = _commands.Find(c => c.Name == commandText);

            if (runMe == null)
            {
                return;
            }

            var commandType = Type.GetType($"KungBot.Twitch.Commands.{runMe.Identifier}Command");

            if (!(Activator.CreateInstance(commandType) is ICommand command))
            {
                return;
            }

            command.IsActive = runMe.IsActive;
            command.Name = runMe.Name;
            command.AuthorizeLevel = runMe.AuthorizationLevel;
            command.Identifier = runMe.Identifier;

            var commandMethod = commandType.GetMethod(runMe.Instructions);

            commandMethod.Invoke(command, new object[] { _client, _twitchService, e.Command, runMe });
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
            HandleNewSubscriber(sender, e, _viewerCollection.GetAsync("viewer-username", e.Subscriber.DisplayName.ToLowerInvariant()).GetAwaiter().GetResult().FirstOrDefault()?.Value).GetAwaiter().GetResult();
            _client.SendMessage(e.Channel,
                e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}!");
        }

        public static async Task HandleNewSubscriber(object sender, OnNewSubscriberArgs onNewSubscriberArgs, Viewer dbViewer = null)
        {

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
            var isSub = (onSub != null) ? true : onMessage.ChatMessage.IsSubscriber;
            var subMonthCount = onMessage?.ChatMessage.SubscribedMonthCount ?? 0;

            var dbRows = (await _viewerCollection.GetAsync("viewer-username", username)).ToList();

            if (dbRows.Any()) return dbRows.First().Value;

            client.SendMessage(channel, $"kungraHYPERS {username}, welcome to the stream!");
            var viewer = new Viewer()
            {
                Username = username,
                IsSubscriber = isSub,
                Points = _account.TwitchBotSettings.DefaultExperienceAmount,
                SubscribedMonthCount = subMonthCount
            };

            return await _viewerCollection.AddOrUpdateAsync(viewer);

        }

        public static void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Channel}");
        }
    }
}
