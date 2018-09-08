using Data.Enumerations;

namespace Data.Models
{
    public class Viewer
    {
        public string Username { get; set; }
        public bool IsFollower { get; set; }
        public bool IsSubscriber { get; set; }
        public int SubscribedMonthCount { get; set; }
        public bool IsBanned { get; set; }
        public int Points { get; set; }
        public SubscriptionLevel SubscriptionLevel { get; set; }
    }
}