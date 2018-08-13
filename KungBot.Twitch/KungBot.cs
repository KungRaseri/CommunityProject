using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;
using TwitchLib.PubSub;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly Settings _appSettings;
        private readonly TwitchClient _client;
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

            TwitchHandlers.Init(_client, _twitchPubSub, _appSettings, _viewerCollection, _commands);
        }

        public void Disconnect()
        {
            _client.ChatThrottler.StopQueue();
            _client.Disconnect();
        }
    }
}
