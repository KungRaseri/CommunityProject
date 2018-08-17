using System;
using System.Linq;
using System.Xml.Linq;
using Data.Helpers;
using Data.Models;
using Syn.Bot.Channels.Discord;
using Syn.Bot.Oscova;

namespace KungBot.Discord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appSettingsCollection = new CouchDbStore<ApplicationSettings>(ApplicationSettings.CouchDbUrl);
            var userCollection = new CouchDbStore<User>(ApplicationSettings.CouchDbUrl);

            var appSettings = appSettingsCollection.GetAsync().Result.FirstOrDefault()?.Value;
            var userSettings = userCollection.GetAsync("")

            if (appSettings == null || userSettings == null)
            {
                Console.WriteLine("Settings could not be found...");
                return;
            }

            var app = new KungBot(appSettings, userSettings);

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