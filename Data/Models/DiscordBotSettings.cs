using System.Collections.Generic;

namespace Data.Models
{
    public class DiscordBotSettings
    {
        public string CommandCharacter { get; set; }
        public string ChannelName { get; set; }
        public string BotNickname { get; set; }
        public List<Command> Commands { get; set; }
    }
}