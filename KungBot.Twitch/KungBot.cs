using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using KungBot.Twitch.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using RestSharp;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly Settings _settings;
        private readonly TwitchClient _client;
        private readonly ILogger<TwitchClient> _logger;
        private readonly TwitchService _twitchService;
        private readonly List<Command> _commands;

        public KungBot()
        {
            var settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984"); // LEAKED
            _settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;

            var commandCollection = new CouchDbStore<Command>(_settings?.CouchDbUri);
            _commands = commandCollection.GetAsync().Result.Select(row => row.Value).ToList();

            var factory = new LoggerFactory(new List<ILoggerProvider>()
            {
                new ConsoleLoggerProvider(new ConsoleLoggerSettings(){Switches = {new KeyValuePair<string, LogLevel>("KungTheBot", LogLevel.Debug) }})
            });
            _logger = new Logger<TwitchClient>(factory);//new ConsoleLogger();
            _client = new TwitchClient(_logger);

            _twitchService = new TwitchService(_settings);
        }

        public void Connect()
        {
            Task.Run(InitializeBot);
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commands.Count} commands");
            _client.Connect();
        }

        private async Task InitializeBot()
        {
            var credentials = new ConnectionCredentials(_settings?.TwitchBotSettings.Username, _settings?.Keys.Twitch.Bot.Oauth);

            _client.Initialize(credentials, "KungRaseri", autoReListenOnExceptions: false);
            _client.ChatThrottler = new MessageThrottler(_client, 20, TimeSpan.FromSeconds(30));
            await _client.ChatThrottler.StartQueue();
            _client.WhisperThrottler = new MessageThrottler(_client, 20, TimeSpan.FromSeconds(30));

            if (_settings != null) _client.AddChatCommandIdentifier(_settings.TwitchBotSettings.CommandCharacter);

            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnNewSubscriber += OnNewSubscriber;
            _client.OnLog += OnLog;
            _client.OnConnectionError += OnConnectionError;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.OnUserTimedout += OnUserTimedOut;
            _client.OnUserBanned += ClientOnOnUserBanned;
        }

        private async void ClientOnOnUserBanned(object sender, OnUserBannedArgs e)
        {
            var client = new RestClient("http://localhost:57463/ws/api/botcommandrelay");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.Username} has been banned. Reason: {e.BanReason} Kappa");

            await client.ExecuteGetTaskAsync(request);
        }

        private async void OnUserTimedOut(object sender, OnUserTimedoutArgs e)
        {
            var client = new RestClient("http://localhost:57463/ws/api/botcommandrelay");
            var request = new RestRequest(Method.GET);
            request.AddQueryParameter("command", "timeout");
            request.AddQueryParameter("message", $"{e.Username} timed out for {e.TimeoutDuration} seconds. Reason: {e.TimeoutReason} Kappa");

            await client.ExecuteGetTaskAsync(request);
        }

        private void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
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
                e.Subscriber.IsTwitchPrime
                    ? $"Welcome {e.Subscriber.DisplayName} to the {_settings.TwitchBotSettings.CommunityName}! You just earned {_settings.TwitchBotSettings.NewSubAwardAmount} {_settings.TwitchBotSettings.PointsName}! May the Lords bless you for using your Twitch Prime!"
                    : $"Welcome {e.Subscriber.DisplayName} to the {_settings.TwitchBotSettings.CommunityName}! You just earned {_settings.TwitchBotSettings.NewSubAwardAmount} {_settings.TwitchBotSettings.PointsName}!");
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //keyword matching
            //blacklist
            //whitelist
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
