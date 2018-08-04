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
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly Settings _appSettings;
        private readonly TwitchClient _client;
        private readonly TwitchService _twitchService;
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
            _client = new TwitchClient(logger: logger);

            _twitchService = new TwitchService(_appSettings);
        }

        public async Task Connect()
        {
            await InitializeBot();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commands.Count} commands");
            _client.Connect();
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

        private async Task HandleViewerExperience(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.IsBroadcaster)
                return;

            var dbViewer = (await _viewerCollection.GetAsync("viewer-username", e.ChatMessage.Username)).FirstOrDefault()?.Value;

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

            _client.SendMessage(e.Channel,
                e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_appSettings.TwitchBotSettings.CommunityName}! You just earned {_appSettings.TwitchBotSettings.NewSubAwardAmount} {_appSettings.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_appSettings.TwitchBotSettings.CommunityName}! You just earned {_appSettings.TwitchBotSettings.NewSubAwardAmount} {_appSettings.TwitchBotSettings.PointsName}!");
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            HandleNewViewer(sender, e).GetAwaiter().GetResult();
            HandleViewerExperience(sender, e).GetAwaiter().GetResult();

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

        private async Task HandleNewViewer(object sender, OnMessageReceivedArgs e)
        {
            var client = (TwitchClient)sender;

            var dbRows = await _viewerCollection.GetAsync("viewer-username", e.ChatMessage.Username);

            if (!dbRows.Any())
            {
                client.SendMessage(e.ChatMessage.Channel, $"kungraHYPERS {e.ChatMessage.DisplayName}, welcome to the stream!");
                var viewer = new Viewer()
                {
                    Username = e.ChatMessage.Username,
                    IsSubscriber = e.ChatMessage.IsSubscriber,
                    Experience = _appSettings.TwitchBotSettings.DefaultExperienceAmount
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
