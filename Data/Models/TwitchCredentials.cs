namespace Data.Models
{
    public partial class TwitchCredentials
    {
        public BotCredentials Bot { get; set; }
        public string ChannelId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
