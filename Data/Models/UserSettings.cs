using Data.Interfaces;

namespace Data.Models
{
    public class UserSettings : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public TwitchBotSettings TwitchBotSettings { get; set; }
        public DiscordBotSettings DiscordBotSettings { get; set; }
    }
}
