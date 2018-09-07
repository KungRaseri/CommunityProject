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
        private readonly TwitchPubSub _twitchPubSub;
        private readonly List<Command> _commandSettings;
        private readonly CouchDbStore<Viewer> _viewerCollection;
        private ChannelManager _channelManager;

        public KungBot(TwitchPubSub twitchPubSub, ChannelManager channelManager)
        {
            _twitchPubSub = twitchPubSub;
            _channelManager = channelManager;
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
            InitializeBot();
            Console.WriteLine("Connecting...");
            Console.WriteLine($"Loaded {_commandSettings.Count} commands");
            _twitchPubSub.Connect();
            _channelManager.Connect();
            Console.WriteLine($"Connected...");
        }

        private void InitializeBot()
        {
            _channelManager.Init();
            TwitchHandlers.Init(_channelManager, _twitchPubSub, _appSettings, _account, _viewerCollection, _commandSettings);
        }

        public void Disconnect()
        {
            _channelManager.Disconnect();
        }
    }
}