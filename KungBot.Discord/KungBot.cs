using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Net.WebSocket;
using DSharpPlus.VoiceNext;
using DSharpPlus.VoiceNext.Codec;
using KungBot.Discord.Commands;
using KungBot.Discord.Voice;
using Syn.Bot.Channels.Discord;
using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Events;

namespace KungBot.Discord
{
    public class KungBot
    {
        private DiscordClient Client;
        private CommandsNextModule CommandsNext { get; set; }
        private DiscordChannel<OscovaBot> _discordChannel;
        private OscovaBot _oscovaBot;
        private readonly Settings _settings;
        private VoiceNextClient _voiceNextClient;

        public static string CommandCharacter;

        public KungBot(Settings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            var config = new DiscordConfiguration()
            {
                Token = _settings.Keys.Discord,
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
            _voiceNextClient = Client.UseVoiceNext(vnConfig);

            CommandCharacter = _settings.DiscordBotSettings.CommandCharacter;

            Client.SetWebSocketClient<WebSocket4NetCoreClient>();
            Client.Ready += ClientOnReady;
            Client.GuildAvailable += ClientOnGuildAvailable;
            Client.SocketOpened += ClientOnSocketOpened;
            Client.ClientErrored += ClientOnClientErrored;
            Client.MessageCreated += ClientOnMessageCreated;
            Client.MessageUpdated += ClientOnMessageUpdated;
            Client.SocketClosed += ClientOnSocketClosed;

            //_oscovaBot = new OscovaBot();
            //_discordChannel = new DiscordChannel<OscovaBot>(_oscovaBot, _settings.Keys.Discord);

            //_oscovaBot.Import(XDocument.Load("knowledge.siml"));
            ////_oscovaBot.Import(XDocument.Load("salutations.siml"));

            //_oscovaBot.Trainer.StartTraining();

            //_oscovaBot.MainUser.ResponseReceived += MainUserOnResponseReceived;

            //_discordChannel.Start().Wait();
        }

        public void Configure()
        {
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = CommandCharacter,
                EnableMentionPrefix = true
            });

            CommandsNext.CommandExecuted += CommandsNextOnCommandExecuted;
            CommandsNext.CommandErrored += CommandsNextOnCommandErrored;

            CommandsNext.RegisterCommands<DungeonMasterCommands>();
            CommandsNext.RegisterCommands<TwitchCommands>();
            CommandsNext.RegisterCommands<DiscJockeyCommands>();
            CommandsNext.RegisterCommands<HumbleBundleCommands>();
            CommandsNext.RegisterCommands<JanitorCommands>();
            CommandsNext.RegisterCommands<VoiceCommands>();
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

        private async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var channel = await Client.GetChannelAsync(143115828383055872);
                    await Client.SendMessageAsync(channel, Encoding.UTF8.GetString(buffer, 0, result.Count));

                    Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }

                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}
