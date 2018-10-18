using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.PubSub;

namespace KungBot.Twitch
{
    public class KungBot
    {
        private readonly ApplicationSettings _appSettings;
        private readonly Account _account;
        private readonly CouchDbStore<Account> _accountCollection;
        private readonly TwitchClient _client;
        private readonly TwitchPubSub _twitchPubSub;
        private readonly List<Command> _commands;

        public KungBot(TwitchClient client, TwitchPubSub twitchPubSub)
        {
            _client = client;
            _twitchPubSub = twitchPubSub;

            var appSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            _accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);

            _appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            _account = _accountCollection.GetAsync().Result.FirstOrDefault()?.Value;

            _commands = _account?.TwitchBotSettings.Commands ?? new List<Command>();
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
            var credentials = new ConnectionCredentials(_account?.TwitchBotSettings.Username,
                _appSettings?.Keys.Twitch.Bot.Oauth);
            _client.Initialize(credentials, "KungRaseri", autoReListenOnExceptions: false);
            
            if (_account == null) throw new Exception("SHITS BROKE");

            if (_appSettings != null) _client.AddChatCommandIdentifier(_account.TwitchBotSettings.CommandCharacter);

            TwitchHandlers.Init(_client, _twitchPubSub, _appSettings, _account, _commands, _accountCollection);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}