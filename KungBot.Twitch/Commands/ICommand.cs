using System.Collections.Generic;
using Data;
using Data.Enumerations;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public interface ICommand
    {
        void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command);
    }
}
