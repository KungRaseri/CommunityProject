using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Helpers;
using Data.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using KungBot.Discord.Discord.Commands;

namespace KungBot.Discord
{
    public static class Program
    {
        private static DiscordShardedClient Client;
        private static ClientWebSocket _webSocket;
        private static CommandsNextModule CommandsNext { get; set; }

        public static void Main(string[] args)
        {
            var settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984"); // LEAKED
            var settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;

            if (settings == null)
                throw new ArgumentNullException(nameof(settings), "Could not find settings");

            _webSocket = new ClientWebSocket();
            try
            {
                Configure(settings.Keys.Discord, settings.DiscordBotSettings.CommandCharacter);
                Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Configuration failed. \n{e.Message}\n{e.StackTrace}");
            }

            Task.Run(RunBotAsync);

            Console.ReadLine();
        }

        private static void Initialize()
        {
            Client.SocketOpened += ClientOnSocketOpened;
        }

        private static Task ClientOnSocketOpened()
        {
            return Task.CompletedTask;
        }

        private static void Configure(string discordToken, string commandCharacter)
        {
            var config = new DiscordConfiguration()
            {
                Token = discordToken,
                TokenType = TokenType.Bot,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordShardedClient(config);
            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefix = commandCharacter
            }).FirstOrDefault().Value;

            CommandsNext.RegisterCommands<TwitchCommands>();
        }

        public static async Task RunBotAsync()
        {
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:57463/botcommandrelay"), CancellationToken.None);

            Console.WriteLine("Connected!");

            var sending = Task.Run(async () =>
            {
                string line;
                while ((line = Console.ReadLine()) != null && line != String.Empty)
                {
                    var bytes = Encoding.UTF8.GetBytes(line);

                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            });

            var receiving = Receiving(client);

            await Task.WhenAll(sending, receiving);
        }

        private static async Task Receiving(ClientWebSocket client)
        {
            var buffer = new byte[1024 * 4];

            while (true)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var channel = await Client.ShardClients.FirstOrDefault().Value.GetChannelAsync(143115828383055872);
                    await Client.ShardClients.FirstOrDefault().Value.SendMessageAsync(channel, Encoding.UTF8.GetString(buffer, 0, result.Count));

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
