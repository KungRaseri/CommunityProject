using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Commands;
using Data.Helpers;
using Data.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using ThirdParty;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Bot
{
    internal class KungRaseriBot
    {
        public static Settings Settings { get; set; }

        private static DiscordShardedClient _discord;
        private static TwitchBotService _twitch;
        private static LiveStreamMonitor _liveStreamMonitor;
        private static CouchDbStore<Settings> _settingsCollection;
        private static CommandsNextModule _commands;

        public static void Main(string[] args)
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984"); // LEAKED
            Settings = _settingsCollection.GetAsync().GetAwaiter().GetResult().FirstOrDefault()?.Value;

            try
            {
                ConfigureBots();
                StartTwitchBot();
                ConfigureEvents();
                _liveStreamMonitor.SetStreamsByUsername(new List<string>() { "kungraseri" });

            }
            catch (Exception e)
            {
                Console.WriteLine($"Configuration failed. You are a failure.\n{e.Message}\n{e.StackTrace}");
                Console.ReadLine();

                return;
            }

            //StartDiscordBotAsync(_discord).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.ReadLine();
        }

        private static void ConfigureEvents()
        {
            _twitch.Client.OnBeingHosted += OnBeingHosted;
            _twitch.Client.OnHostLeft += OnHostLeft;
            _twitch.Client.OnNowHosting += OnNowHosting;
            _twitch.Client.OnConnected += OnConnected;
            _twitch.Client.OnJoinedChannel += OnJoinChannel;
            _twitch.Client.OnLeftChannel += OnLeftChannel;
            _twitch.Client.OnDisconnected += OnDisconnected;
            _twitch.Client.OnNewSubscriber += OnNewSubscriber;
            _twitch.Client.OnReSubscriber += OnReSubscriber;
            _twitch.Client.OnGiftedSubscription += OnGiftedSubscription;
            _twitch.Client.OnUserBanned += OnUserBanned;
            _twitch.Client.OnUserJoined += OnUserJoined;
            _twitch.Client.OnUserLeft += OnUserLeft;
            _twitch.Client.OnUserTimedout += OnUserTimedOut;
            _twitch.Client.OnWhisperCommandReceived += OnWhisperCommandReceived;
            _twitch.Client.OnChatCommandReceived += OnChatCommandReceived;
        }

        public static async Task StartDiscordBotAsync(DiscordShardedClient discord)
        {
            await discord.StartAsync();

            _liveStreamMonitor.StartService();
            _liveStreamMonitor.OnStreamOnline += LiveStreamMonitorOnOnStreamOnline;
            _liveStreamMonitor.OnStreamOffline += LiveStreamMonitorOnOnStreamOffline;
            //_liveStreamMonitor.OnStreamUpdate += LiveStreamMonitorOnOnStreamUpdate;

            await Task.Delay(-1);
        }

        //private static void LiveStreamMonitorOnOnStreamUpdate(object sender, OnStreamUpdateArgs onStreamUpdateArgs)
        //{
        //    var client = _discord.ShardClients.FirstOrDefault().Value;
        //    var message = client.SendMessageAsync(client.GetChannelAsync(380793762684731392).GetAwaiter().GetResult(), $"{onStreamUpdateArgs.Channel} has been updated: {onStreamUpdateArgs.Stream.Game}!").GetAwaiter().GetResult();
        //}

        private static void LiveStreamMonitorOnOnStreamOffline(object sender, OnStreamOfflineArgs onStreamOfflineArgs)
        {
            var client = _discord.ShardClients.FirstOrDefault().Value;
            var message = client.SendMessageAsync(client.GetChannelAsync(380793762684731392).GetAwaiter().GetResult(), $"{onStreamOfflineArgs.Channel} is no longer live!").GetAwaiter().GetResult();

        }

        private static void LiveStreamMonitorOnOnStreamOnline(object sender, OnStreamOnlineArgs onStreamOnlineArgs)
        {
            var client = _discord.ShardClients.FirstOrDefault().Value;
            var message = client.SendMessageAsync(client.GetChannelAsync(380793762684731392).GetAwaiter().GetResult(), $"{onStreamOnlineArgs.Channel} just went live!").GetAwaiter().GetResult();
        }

        public static void StartTwitchBot()
        {
            _twitch.Connect();
            _twitch.JoinChannel("KungRaseri");
        }

        private static void ConfigureBots()
        {
            if (Settings == null)
            {
                // implement logging
                return;
            }

            //_liveStreamMonitor = new LiveStreamMonitor(new TwitchAPI(), clientId: Settings.Keys.Twitch.ClientId);

            _discord = new DiscordShardedClient(new DiscordConfiguration
            {
                Token = Settings?.Keys.Discord,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            });

            _commands = _discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = Settings.CommandCharacter,
            }).FirstOrDefault().Value;

            _commands.RegisterCommands<TwitchCommands>();
            _commands.RegisterCommands<DungeonMasterCommands>();
            _commands.RegisterCommands<JanitorCommands>();

            var credentials = new ConnectionCredentials("KungRaseri", "biux90vq6aosq2mr881xugyzv5bk4u");

            _twitch = new TwitchBotService(credentials);
        }

        #region Events 

        private static void OnHostLeft(object sender, EventArgs e)
        {
            Console.WriteLine("OnHostLeft");
        }

        public static void OnBeingHosted(object sender, OnBeingHostedArgs e)
        {
            Console.WriteLine("OnBeingHosted");
        }

        private static void OnNowHosting(object sender, OnNowHostingArgs e)
        {
            Console.WriteLine("OnNowHosting");
        }

        private static void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("OnConnected");
        }

        private static void OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            Console.WriteLine("OnGiftedSubscription");
        }

        private static void OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            Console.WriteLine("OnReSubscriber");
        }

        private static void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            Console.WriteLine("OnNewSubscriber");
        }

        private static void OnUserTimedOut(object sender, OnUserTimedoutArgs e)
        {
            Console.WriteLine("OnUserTimedOut");
        }

        private static void OnUserLeft(object sender, OnUserLeftArgs e)
        {
            Console.WriteLine("OnUserLeft");
        }

        private static void OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine("OnUserJoined");
        }

        private static void OnUserBanned(object sender, OnUserBannedArgs e)
        {
            Console.WriteLine("OnUserBanned");
        }

        private static void OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            Console.WriteLine("OnDisconnected");
        }

        private static void OnLeftChannel(object sender, OnLeftChannelArgs e)
        {
            Console.WriteLine("OnLeftChannel");
        }

        private static void OnJoinChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("OnJoinChannel");
        }

        private static void OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {
            Console.WriteLine("OnChatCommandReceived");
        }

        private static void OnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
        {
            Console.WriteLine("OnWhisperCommandReceived");
        }

        #endregion

    }
}