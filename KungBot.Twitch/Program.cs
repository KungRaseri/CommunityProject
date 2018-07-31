using System;
using System.Threading;

namespace KungBot.Twitch
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var bot = new KungBot();
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
    }
}
