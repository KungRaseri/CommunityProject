using System;
using System.Collections.Generic;
using System.Text;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class LogErrorCommand : ICommand
    {
        public string GetKey()
        {
            return "LogError";
        }

        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            // Log error
            throw new NotImplementedException();
        }
    }
}
