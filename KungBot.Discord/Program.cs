using System;
using System.Linq;
using Data.Helpers;
using Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace KungBot.Discord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);

            var appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            var account = accountCollection.GetAsync().Result.FirstOrDefault()?.Value;

            if (appSettings == null || account == null)
            {
                Console.WriteLine("Settings could not be found...");
                return;
            }

            var app = new KungBot(appSettings, account);

            try
            {
                app.Initialize();
                app.Configure();
            }
            catch (Exception e)
            {
                if (e.TargetSite.Name.Contains("initialize"))
                    Console.WriteLine($"Initialization failed. \n{e.Message}\n{e.StackTrace}");
                else if (e.TargetSite.Name.Contains("configure"))
                    Console.WriteLine($"Configuration failed. \n{e.Message}\n{e.StackTrace}");
                else
                    Console.WriteLine($"Unknown failure. \n{e.Message}\n{e.StackTrace}");

            }

            app.RunBotAsync().GetAwaiter().GetResult();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            Console.Write("test");
        }
    }
}