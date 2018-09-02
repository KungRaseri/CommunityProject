using System;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using KungBot.Discord.Commands;
using Syn.Bot.Oscova.Events;

namespace KungBot.Discord
{
    public class KungBot
    {
        private DiscordClient Client;
        private CommandsNextModule CommandsNext { get; set; }
        private readonly ApplicationSettings _appSettings;
        private readonly Account _account;

        public KungBot(ApplicationSettings appSettings, Account account)
        {
            _appSettings = appSettings;
            _account = account;
        }

        public void Initialize()
        {
            var config = new DiscordConfiguration()
            {
                Token = _appSettings.Keys.Discord,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            var vnConfig = new VoiceNextConfiguration()
            {
                VoiceApplication = VoiceApplication.Music,
                EnableIncoming = false
            };


            Client = new DiscordClient(config);
            Client.UseVoiceNext(vnConfig);

            Client.SetWebSocketClient<WebSocket4NetCoreClient>();
            Client.Ready += ClientOnReady;
            Client.GuildAvailable += ClientOnGuildAvailable;
            Client.SocketOpened += ClientOnSocketOpened;
            Client.ClientErrored += ClientOnClientErrored;
            Client.MessageCreated += ClientOnMessageCreated;
            Client.MessageUpdated += ClientOnMessageUpdated;
            Client.SocketClosed += ClientOnSocketClosed;
        }

        public void Configure()
        {
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = _account.DiscordBotSettings.CommandCharacter,
                EnableMentionPrefix = true
            });

            CommandsNext.CommandExecuted += CommandsNextOnCommandExecuted;
            CommandsNext.CommandErrored += CommandsNextOnCommandErrored;

            CommandsNext.RegisterCommands<DungeonMasterCommands>();
            CommandsNext.RegisterCommands<TwitchCommands>();
            CommandsNext.RegisterCommands<DiscJockeyCommands>();
            CommandsNext.RegisterCommands<HumbleBundleCommands>();
            CommandsNext.RegisterCommands<JanitorCommands>();
            //CommandsNext.RegisterCommands<VoiceCommands>();
        }

        private void MainUserOnResponseReceived(object sender, ResponseReceivedEventArgs e)
        {
            Client.DebugLogger.LogMessage(LogLevel.Debug, Client.CurrentApplication.Name, e.Response.Text, DateTime.UtcNow);
        }

        private Task ClientOnSocketClosed(SocketCloseEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task ClientOnMessageUpdated(MessageUpdateEventArgs e)
        {
            //e.Message.CreateReactionAsync(e.Guild.Emojis[new Random().Next(0, e.Guild.Emojis.Count - 1)]);
            return Task.CompletedTask;
        }

        private Task ClientOnMessageCreated(MessageCreateEventArgs e)
        {
            if (!e.MentionedUsers.Any(du => du.Username.Equals("KungBot")))
            {
                return Task.CompletedTask;
            }

            //if (e.Message.Content.ToLower().Contains("hey") || e.Message.Content.ToLower().Contains("mehdio"))
            //    e.Channel.SendMessageAsync(
            //        $"{e.Guild.GetEmojisAsync().Result.FirstOrDefault(emoji => emoji.Name.Contains("kungHEY"))?.ToString()} {e.Author.Mention}");

            return Task.CompletedTask;
        }

        private Task ClientOnClientErrored(ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, e.Client.CurrentApplication.Name, e.Exception.Message, DateTime.UtcNow);

            return Task.CompletedTask;
        }

        private Task ClientOnGuildAvailable(GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, e.Client.CurrentApplication.Name, "Guild available", DateTime.UtcNow);

            return Task.CompletedTask;
        }

        private Task ClientOnSocketOpened()
        {
            Client.DebugLogger.LogMessage(LogLevel.Debug, Client.CurrentApplication.Name, "Socket Opened", DateTime.UtcNow);
            return Task.CompletedTask;
        }

        private Task ClientOnReady(ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Debug, e.Client.CurrentApplication.Name, "Client ready", DateTime.UtcNow);

            return Task.CompletedTask;
        }

        private Task CommandsNextOnCommandErrored(CommandErrorEventArgs e)
        {
            return Task.CompletedTask;
        }

        private Task CommandsNextOnCommandExecuted(CommandExecutionEventArgs e)
        {
            return Task.CompletedTask;
        }

        public async Task RunBotAsync()
        {
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
