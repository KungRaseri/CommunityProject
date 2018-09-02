using System.Collections.Generic;
using Data.Interfaces;

namespace Data.Models
{
    public class Account : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public TwitchBotSettings TwitchBotSettings { get; set; }
        public DiscordBotSettings DiscordBotSettings { get; set; }
        public List<Viewer> Viewers { get; set; }
        public List<ViewerRank> ViewerRanks { get; set; }
    }
}
