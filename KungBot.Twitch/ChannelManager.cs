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
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;

namespace KungBot.Twitch
{
    public class ChannelManager
    {
        private readonly TwitchClient _client;
        private readonly ApplicationSettings _appSettings;
        private readonly Account _account;
        private readonly CouchDbStore<Viewer> _viewerCollection;
        private readonly List<Command> _commandSettings;
        private readonly TwitchService _twitchService;

        public ChannelManager(TwitchClient client, CouchDbStore<ApplicationSettings> appSettingsCollection, 
            CouchDbStore<Account> accountCollection, CouchDbStore<Viewer> viewerCollection, CouchDbStore<Command> commandSettingsCollection)
        {
            _client = client;
            _appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _account = accountCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _viewerCollection = viewerCollection;
            _commandSettings = commandSettingsCollection.GetAsync().Result.Select(row => row.Value).ToList();;
            _twitchService = new TwitchService(_appSettings);
        }

        public async void Init()
        {
            var credentials = new ConnectionCredentials(_account?.TwitchBotSettings.Username,
                _appSettings?.Keys.Twitch.Bot.Oauth);

            _client.Initialize(credentials, autoReListenOnExceptions: false);
            _client.ChatThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));
            _client.WhisperThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));

            await _client.ChatThrottler.StartQueue();
            await _client.WhisperThrottler.StartQueue();

            if (_appSettings != null) _client.AddChatCommandIdentifier(_account.TwitchBotSettings.CommandCharacter);

            ClientHandlers();
        }

        private void ClientHandlers()
        {
            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnNewSubscriber += OnNewSubscriber;
            _client.OnLog += OnLog;
            _client.OnConnectionError += OnConnectionError;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.OnUserTimedout += OnUserTimedOut;
            _client.OnUserBanned += ClientOnUserBanned;
        }

        public void Connect()
        {
            _client.Connect();
        }

        public void Disconnect()
        {
            _client.ChatThrottler.StopQueue();
            _client.Disconnect();
        }

        public void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Channel}");
        }
        
        public void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleViewerExperience(sender, e).GetAwaiter().GetResult();

            //keyword matching
            HandleKeywordMatching(sender, e);
            //blacklist
            HandleBlacklistMatching(sender, e);
            //whitelist
            HandleWhitelistMatching(sender, e);
        }

        public void HandleWhitelistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        public void HandleBlacklistMatching(object sender, OnMessageReceivedArgs e)
        {

        }

        public void HandleKeywordMatching(object sender, OnMessageReceivedArgs e)
        {

        }
        
        public async Task HandleViewerExperience(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsBroadcaster)
                return;

            var dbViewer = await HandleNewViewer(sender, e);

            dbViewer.IsSubscriber = e.ChatMessage.IsSubscriber;
            dbViewer.Points += _account.TwitchBotSettings.DefaultExperienceAmount;

            await _viewerCollection.AddOrUpdateAsync(dbViewer);
        }

        public async Task<Viewer> HandleNewViewer(object sender, EventArgs e)
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

        public void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!");
        }
        public void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            HandleNewSubscriber(sender, e, _viewerCollection.GetAsync("viewer-username", e.Subscriber.DisplayName.ToLowerInvariant()).GetAwaiter().GetResult().FirstOrDefault()?.Value).GetAwaiter().GetResult();
            _client.SendMessage(e.Channel,
                e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_account.TwitchBotSettings.CommunityName}! You just earned {_account.TwitchBotSettings.NewSubAwardAmount} {_account.TwitchBotSettings.PointsName}!");
        }
        public async Task HandleNewSubscriber(object sender, OnNewSubscriberArgs onNewSubscriberArgs, Viewer dbViewer = null)
        {

        }
        public void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: \n{e.Error.Exception}\n{e.Error.Message}");
        }

        public void OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.BotUsername}: {e.Data}");
        }
        public void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            CheckForCommand(sender, e);
        }

        public void CheckForCommand(object sender, OnChatCommandReceivedArgs e)
        {
            var commandText = e.Command.CommandText;

            var commandSettings = _commandSettings.Find(c => c.Name == commandText);

            if (commandSettings == null) return;
            if (!commandSettings.IsActive) CommandUtility.GetCommandByKey("LogError")(_client, _twitchService, e.Command, commandSettings);

            CommandUtility.GetCommandByKey(commandSettings.Identifier)(_client, _twitchService, e.Command, commandSettings);
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

        public TwitchClient GetClient()
        {
            return _client;
        }
    }
}
