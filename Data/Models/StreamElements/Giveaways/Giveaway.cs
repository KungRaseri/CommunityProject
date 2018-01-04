using System;
using System.Collections.Generic;

namespace Data.Models.StreamElements.Giveaways
{
    public class Giveaway
    {
        public string _id { get; set; }
        public string Title { get; set; }
        public string Channel { get; set; }
        public int Cost { get; set; }
        public int MaxTickets { get; set; }
        public object Preview { get; set; }
        public string Description { get; set; }
        public IEnumerable<GiveawayWinner> Winners { get; set; }
        public IEnumerable<string> Entries { get; set; }
        public bool BotResponses { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAmount { get; set; }
        public int SubscriberLuck { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
