using System.Collections.Generic;
using Data.Interfaces;

namespace Data.Models
{
    public class TwitchBotSettings
    {
        public char CommandCharacter { get; set; }
        public string CommunityName { get; set; }
        public string NewSubAwardAmount { get; set; }
        public string PointsName { get; set; }
        public string Username { get; set; }
        public int DefaultExperienceAmount { get; set; }
        public List<Command> Commands { get; set; }
    }
}
