using System;
using System.Linq;
using Data.Helpers;
using Data.Models;

namespace KungBot.Discord
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var appSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var accountCollection = new CouchDbStore<Account>(ApplicationSettings.CouchDbUrl);

            var appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            var account = accountCollection.GetAsync("account-email", "test@email.test.com").Result.FirstOrDefault()?.Value;

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
                Console.WriteLine($"Configuration failed. \n{e.Message}\n{e.StackTrace}");
            }

            app.RunBotAsync().GetAwaiter().GetResult();
        }
    }
}