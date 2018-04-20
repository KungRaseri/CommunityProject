using System;
using System.Threading;

namespace KungBot.Twitch
{
    public class Program
    {
        protected static void Main(string[] args)
        {
            var bot = new KungBot();
            bot.Connect();
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
    }
}
