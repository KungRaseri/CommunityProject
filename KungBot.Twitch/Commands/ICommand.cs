using System.Collections.Generic;
using Data;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public interface ICommand
    {
        string Name { get; set; }
        string Identifier { get; set; }
        AuthLevel AuthorizeLevel { get; set; }
        void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command);
    }
}
