using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Commands;
using Data.Helpers;
using Data.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using TwitchLib;
using TwitchLib.Events.Services.LiveStreamMonitor;
using TwitchLib.Services;

namespace Bot
{
    internal class KungRaseriBot
    {
        public static Settings Settings { get; set; }

        private static DiscordShardedClient _discord;
        private static TwitchClient _twitch;
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
                _liveStreamMonitor.SetStreamsByUsername(new List<string>() { "blazdnconfuzd" });

            }
            catch (Exception e)
            {
                Console.WriteLine($"Configuration failed. You are a failure.\n{e.Message}\n{e.StackTrace}");
                Console.ReadLine();

                return;
            }

            //StartTwitchBot();
            StartDiscordBotAsync(_discord).ConfigureAwait(false).GetAwaiter().GetResult();
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
        }

        private static void ConfigureBots()
        {
            if (Settings == null)
            {
                // implement logging
                return;
            }

            _liveStreamMonitor = new LiveStreamMonitor(new TwitchAPI(Settings.Keys.Twitch.ClientId), clientId: Settings.Keys.Twitch.ClientId);

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
        }
    }
}