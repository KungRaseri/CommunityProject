namespace Data.Models.StreamElements.Giveaways
{
    public class GiveawayWinner
    {
        public string _id { get; set; }
        public int TwitchId { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public int Tickets { get; set; }
    }
}