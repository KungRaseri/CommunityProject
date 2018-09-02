using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Enumerations;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class EmotesCommand : ICommand
    {
        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!command.IsActive)
                return;

            client.SendMessage(chatCommand.ChatMessage.Channel, "Tier 1 emote: kungraHEY Tier 2 emote: kungraDERP Tier 3 emote: kungraTHRONE");
        }
    }
}
