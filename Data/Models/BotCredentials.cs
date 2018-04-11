namespace Data.Models
{
    public partial class TwitchCredentials
    {
        public class BotCredentials
        {
            public string Username { get; set; }
            public string Oauth { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
        }
    }
}
