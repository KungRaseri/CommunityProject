using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Helpers;
using Data.Models;
using KungBot.Twitch.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RestSharp;
using Serilog.Extensions.Logging;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly Settings _appSettings;
        private readonly TwitchClient _client;
        private readonly TwitchService _twitchService;
        private readonly TwitchPubSub _twitchPubSub;
        private readonly List<Command> _commands;
        private readonly CouchDbStore<Viewer> _viewerCollection;

        public KungBot()
        {
            var settingsCollection = new CouchDbStore<Settings>(Settings.CouchDbUrl);
            _appSettings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;

            _viewerCollection = new CouchDbStore<Viewer>(Settings.CouchDbUrl);

            var commandCollection = new CouchDbStore<Command>(Settings.CouchDbUrl);
            _commands = commandCollection.GetAsync().Result.Select(row => row.Value).ToList();

            var factory = new LoggerFactory(new List<ILoggerProvider>()
            {
                new ConsoleLoggerProvider(new ConsoleLoggerSettings(){Switches = {new KeyValuePair<string, LogLevel>("KungTheBot", LogLevel.Debug) }})
            });

            ILogger<TwitchClient> logger = new Logger<TwitchClient>(factory);
            ILogger<TwitchPubSub> pubSubLogger = new Logger<TwitchPubSub>(factory);
            _client = new TwitchClient(logger: logger);

            _twitchService = new TwitchService(_appSettings);
            _twitchPubSub = new TwitchPubSub(pubSubLogger);
        }

        public async Task Connect()
        {
            await InitializeBot();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commands.Count} commands");
            _twitchPubSub.Connect();
            _client.Connect();
            Console.WriteLine($"Connected...");
        }

        private async Task InitializeBot()
        {
            var credentials = new ConnectionCredentials(_appSettings?.TwitchBotSettings.Username, _appSettings?.Keys.Twitch.Bot.Oauth);

            _client.Initialize(credentials, "KungRaseri", autoReListenOnExceptions: false);
            _client.ChatThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));
            _client.WhisperThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));

            await _client.ChatThrottler.StartQueue();
            await _client.WhisperThrottler.StartQueue();

            if (_appSettings != null) _client.AddChatCommandIdentifier(_appSettings.TwitchBotSettings.CommandCharacter);

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
            _twitchPubSub.ListenToChatModeratorActions(_appSettings?.TwitchBotSettings.Username, _appSettings?.Keys.Twitch.ChannelId);
        }

        private void TwitchPubSubOnOnEmoteOnly(object sender, OnEmoteOnlyArgs e)
        {
            _client.SendMessage("kungraseri", $"emote mode on, ran by {e.Moderator}!");
        }

        private void TwitchPubSubOnOnEmoteOnlyOff(object sender, OnEmoteOnlyOffArgs e)
        {
            _client.SendMessage("kungraseri", $"emote mode off, ran by {e.Moderator}!");
        }

        private void TwitchPubSubOnOnFollow(object sender, OnFollowArgs e)
        {
            _client.SendMessage("kungraseri", $"Thank you for the follow, {e.DisplayName}!");
        }

        private void TwitchPubSubOnOnPubSubServiceClosed(object sender, EventArgs e)
        {
            _client.SendMessage("kungraseri", $"pubsub service closed");
        }

        private void TwitchPubSubOnOnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
        {
            _client.SendMessage("kungraseri", $"{e.Subscription.DisplayName} just subscribed with a {e.Subscription.SubscriptionPlanName} sub!");
        }

        private void TwitchPubSubOnOnPubSubServiceConnected(object sender, EventArgs e)
        {
            _client.SendMessage("kungraseri", $"connected to the pubsub service");
        }

        private async void ClientOnUserBanned(object sender, OnUserBannedArgs e)
        {
            var client = new RestClient(WebSocketSettings.LocalBotCommandRelayUrl);
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.UserBan.Username} has been banned. Reason: {e.UserBan.BanReason} Kappa");

            await client.ExecuteGetTaskAsync(request);
        }

        private async void OnUserTimedOut(object sender, OnUserTimedoutArgs e)
        {
            var client = new RestClient(WebSocketSettings.LocalBotCommandRelayUrl);
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.UserTimeout.Username} timed out for {e.UserTimeout.TimeoutDuration} seconds. Reason: {e.UserTimeout.TimeoutReason} Kappa");

            await client.ExecuteGetTaskAsync(request);
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            CheckForCommand(sender, e);
        }

        private async Task HandleViewerExperience(object sender, OnMessageReceivedArgs e, Viewer dbViewer = null)
        {
            if (e.ChatMessage.IsBroadcaster)
                return;

            if (dbViewer == null)
                await HandleNewViewer(sender, e);

            dbViewer.IsSubscriber = e.ChatMessage.IsSubscriber;
            dbViewer.Experience += _appSettings.TwitchBotSettings.DefaultExperienceAmount;

            await _viewerCollection.AddOrUpdateAsync(dbViewer);
        }

        private void CheckForCommand(object sender, OnChatCommandReceivedArgs e)
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

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: \n{e.Error.Exception}\n{e.Error.Message}");
        }

        private void OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Data}");
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            HandleNewSubscriber(sender, e, _viewerCollection.GetAsync("viewer-username", e.Subscriber.DisplayName.ToLowerInvariant()).GetAwaiter().GetResult().FirstOrDefault()?.Value).GetAwaiter().GetResult();
            _client.SendMessage(e.Channel,
                e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_appSettings.TwitchBotSettings.CommunityName}! You just earned {_appSettings.TwitchBotSettings.NewSubAwardAmount} {_appSettings.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_appSettings.TwitchBotSettings.CommunityName}! You just earned {_appSettings.TwitchBotSettings.NewSubAwardAmount} {_appSettings.TwitchBotSettings.PointsName}!");
        }

        private async Task HandleNewSubscriber(object sender, OnNewSubscriberArgs onNewSubscriberArgs, Viewer dbViewer = null)
        {

        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleViewerExperience(sender, e, _viewerCollection.GetAsync("viewer-username", e.ChatMessage.Username).GetAwaiter().GetResult().FirstOrDefault()?.Value).GetAwaiter().GetResult();

            //keyword matching
            HandleKeywordMatching(sender, e);
            //blacklist
            HandleBlacklistMatching(sender, e);
            //whitelist
            HandleWhitelistMatching(sender, e);
        }

        private void HandleWhitelistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        private void HandleBlacklistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        private void HandleKeywordMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        private async Task HandleNewViewer(object sender, EventArgs e)
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

            var dbRows = await _viewerCollection.GetAsync("viewer-username", username);

            if (!dbRows.Any())
            {
                client.SendMessage(channel, $"kungraHYPERS {username}, welcome to the stream!");
                var viewer = new Viewer()
                {
                    Username = username,
                    IsSubscriber = isSub,
                    Experience = _appSettings.TwitchBotSettings.DefaultExperienceAmount,
                    SubscribedMonthCount = subMonthCount
                };

                await _viewerCollection.AddOrUpdateAsync(viewer);
            }
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Channel}");
        }

        public void Disconnect()
        {
            _client.ChatThrottler.StopQueue();
            _client.Disconnect();
        }
    }
}
