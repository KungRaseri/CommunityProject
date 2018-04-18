﻿using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Models;
using ThirdParty;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace KungBot.Twitch.Commands
{
    public class EmotesCommand : ICommand
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public AuthLevel AuthorizeLevel { get; set; }
        public bool IsActive { get; set; }

        public void Perform(TwitchClient client, TwitchService service, ChatCommand chatCommand, Command command)
        {
            if (!IsActive)
                return;

            client.SendMessage(chatCommand.ChatMessage.Channel, "Tier 1 emote: kungraHEY Tier 2 emote: kungraDERP Tier 3 emote: kungraTHRONE");
        }
    }
}