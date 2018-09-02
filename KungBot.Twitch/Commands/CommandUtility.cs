using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwitchLib.Client;
using Data.Models;
using TwitchLib.Client.Models;
using ThirdParty;

namespace KungBot.Twitch.Commands
{
    public static class CommandUtility
    {
        private static IEnumerable<ICommand> _commands = new List<ICommand>
        {
            new EmotesCommand(),
            new RankCommand(),
            new UptimeCommand()
        };

        public static Action<TwitchClient, TwitchService, ChatCommand, Command> GetCommandByKey(string key)
        {
            var command = _commands.FirstOrDefault(setting => setting.GetKey().Equals(key));
            if (command != null)
                return command.Perform;
            return GetCommandByKey("LogError");
        }
    }
}
