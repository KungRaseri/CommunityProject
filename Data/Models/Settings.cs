using Data.Interfaces;

namespace Data.Models
{
    public class Settings : IEntity
    {
        public const string StreamElementsAPIUrl = "https://api.streamelements.com/kappa/v2";
        public const string PanelLocalUrl = "http://localhost:8080";
        public const string CouchDbUrl = "http://admin:123456789@localhost:5984/";
        public const string MySqlUrl = "";

        public string _id { get; set; }
        public string _rev { get; set; }
        public Keys Keys { get; set; }
        public string CookieToken { get; set; }
        public TwitchBotSettings TwitchBotSettings { get; set; }
        public DiscordBotSettings DiscordBotSettings { get; set; }
    }
}
