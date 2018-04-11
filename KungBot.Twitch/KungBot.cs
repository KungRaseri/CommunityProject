using System;
using System.Collections.Generic;
using System.Linq;
using Data.Helpers;
using Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MyCouch;
using ThirdParty;
using Tweetinvi.Parameters;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly Settings _settings;
        private static TwitchClient _client;
        private static ILogger<TwitchClient> _logger;
        private static TwitchService _twitchService;
        private readonly List<Command> _commands;

        public KungBot()
        {
            var settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984"); // LEAKED
            _settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;

            var commandCollection = new CouchDbStore<Command>(_settings?.CouchDbUri);
            _commands = commandCollection.GetAsync().Result.Select(row => row.Value).ToList();
        }

        public void Connect()
        {
            var factory = new LoggerFactory(new List<ILoggerProvider>()
            {
                new ConsoleLoggerProvider(new ConsoleLoggerSettings(){Switches = {new KeyValuePair<string, LogLevel>("KungTheBot", LogLevel.Debug) }})
            });
            _logger = new Logger<TwitchClient>(factory);//new ConsoleLogger();
            _client = new TwitchClient(_logger);

            _twitchService = new TwitchService(_settings);

            InitializeBot();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commands.Count} commands");
            _client.Connect();
        }

        private async void InitializeBot()
        {
            var credentials = new ConnectionCredentials(_settings?.Keys.Twitch.Bot.Username, _settings?.Keys.Twitch.Bot.Oauth);

            _client.Initialize(credentials, "KungRaseri", autoReListenOnExceptions: false);
            _client.ChatThrottler = new MessageThrottler(_client, 20, TimeSpan.FromSeconds(30));
            await _client.ChatThrottler.StartQueue();

            _client.WhisperThrottler = new MessageThrottler(_client, 20, TimeSpan.FromSeconds(30));

            _client.OnJoinedChannel += OnJoinedChannel;
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnWhisperReceived += OnWhisperReceived;
            _client.OnNewSubscriber += OnNewSubscriber;
            _client.OnLog += OnLog;
            _client.OnConnectionError += OnConnectionError;
            _client.OnChatCommandReceived += OnChatCommandReceived;
            _client.OnUserTimedout += OnUserTimedOut;
        }

        private void OnUserTimedOut(object sender, OnUserTimedoutArgs e)
        {
            _client.SendMessage(e.Channel, $"{e.Username} timed out for {e.TimeoutDuration} seconds. Reason: {e.TimeoutReason} Kappa");
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
            var command = Activator.CreateInstance(commandType);
            var commandMethod = commandType.GetMethod(runMe.Instructions);

            commandMethod.Invoke(command, new object[] { _client, _twitchService, e.Command, runMe});

            //switch (command)
            //{
            //    case "test":
            //        _client.SendMessage(e.Command.ChatMessage.Channel, "Test Complete. Now leave me alone!");
            //        break;
            //    case "emotes":
            //        _client.SendMessage(e.Command.ChatMessage.Channel, "Tier 1 emote: kungraHEY Tier 2 emote: kungraDERP Tier 3 emote: kungraTHRONE");
            //        break;
            //    case "uptime":
            //        break;
            //    default:
            //        return;
            //}
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
                    ? $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!"
                    : $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        private void OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "KungRaseri")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.ToLower().Contains("hey"))
            {
                _client.SendMessage(e.ChatMessage.Channel, $"HeyGuys");
            }
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {

        }

        public void Disconnect()
        {
            _client.ChatThrottler.StopQueue();
            _client.Disconnect();
        }
    }
}
