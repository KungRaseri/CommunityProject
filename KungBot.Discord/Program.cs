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
            var settingsCollection = new CouchDbStore<Settings>(Settings.CouchDbUrl);
            var settings = settingsCollection.GetAsync().Result.FirstOrDefault()?.Value;

            if (settings == null)
            {
                Console.WriteLine("Settings could not be found...");
                return;
            }

            var app = new KungBot(settings);

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