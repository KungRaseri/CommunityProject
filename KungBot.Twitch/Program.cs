using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using TwitchLib.Client;
using TwitchLib.PubSub;

namespace KungBot.Twitch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var bot = serviceProvider.GetService<KungBot>();
            bot.Connect().GetAwaiter().GetResult();
            try
            {
                while (!Console.ReadLine()?.Contains("exit") ?? true)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            bot.Disconnect();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddSingleton<TwitchClient>()
                    .AddSingleton<TwitchPubSub>()
                    .AddSingleton<KungBot>();
        }
    }
}
