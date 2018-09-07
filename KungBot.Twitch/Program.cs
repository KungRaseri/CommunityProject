using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using Data.Helpers;
using Data.Models;
using Tweetinvi.Core.Events;
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
                    .AddSingleton<ChannelManager>()
                    .AddSingleton(serviceProvider=>new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl))
                    .AddSingleton(serviceProvider=>new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl))
                    .AddSingleton(serviceProvider=>new CouchDbStore<Viewer>(ApplicationSettings.CouchDbUrl))
                    .AddSingleton(serviceProvider=>new CouchDbStore<Command>(ApplicationSettings.CouchDbUrl))
                    .AddSingleton<KungBot>();
        }
    }
}
