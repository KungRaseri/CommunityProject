namespace Data.Models
{
    public class Keys
    {
        public string JWTSecurityKey { get; set; }
        public TwitchCredentials Twitch { get; set; }
        public TwitterCredentials Twitter { get; set; }
        public string Discord { get; set; }
        public string StreamElements { get; set; }
    }
}