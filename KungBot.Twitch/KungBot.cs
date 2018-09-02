using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Services;
using TwitchLib.PubSub;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly ApplicationSettings _appSettings;
        private readonly Account _account;
        private readonly TwitchClient _client;
        private readonly TwitchPubSub _twitchPubSub;
        private readonly List<Command> _commandSettings;
        private readonly CouchDbStore<Viewer> _viewerCollection;

        public KungBot(TwitchClient client, TwitchPubSub twitchPubSub)
        {
            _client = client;
            _twitchPubSub = twitchPubSub;

            var appSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);

            _appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _account = accountCollection.GetAsync().Result.FirstOrDefault()?.Value;

            _viewerCollection = new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl);

            var commandCollection = new CouchDbStore<Command>(ApplicationSettings.CouchDbUrl);
            _commandSettings = commandCollection.GetAsync().Result.Select(row => row.Value).ToList();
        }

        public async Task Connect()
        {
            await InitializeBot();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commandSettings.Count} commands");
            _twitchPubSub.Connect();
            _client.Connect();
            Console.WriteLine($"Connected...");
        }

        private async Task InitializeBot()
        {
            var credentials = new ConnectionCredentials(_account?.TwitchBotSettings.Username,
                _appSettings?.Keys.Twitch.Bot.Oauth);

            _client.Initialize(credentials, "KungRaseri", autoReListenOnExceptions: false);
            _client.ChatThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));
            _client.WhisperThrottler = new MessageThrottler(_client, 15, TimeSpan.FromSeconds(30));

            await _client.ChatThrottler.StartQueue();
            await _client.WhisperThrottler.StartQueue();

            if (_appSettings != null) _client.AddChatCommandIdentifier(_account.TwitchBotSettings.CommandCharacter);

            TwitchHandlers.Init(_client, _twitchPubSub, _appSettings, _account, _viewerCollection, _commandSettings);
        }

        public void Disconnect()
        {
            _client.ChatThrottler.StopQueue();
            _client.Disconnect();
        }
    }
}