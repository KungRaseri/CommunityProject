using System;

namespace Data.Models.StreamElements.LoyaltySettings
{
    public class LoyaltySettings
    {
        public string _id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Channel { get; set; }
        public Loyalty Loyalty { get; set; }
    }

    public class Loyalty
    {
        public Bonuses Bonuses { get; set; }
        public string[] Ignored { get; set; }
        public int SubscriberMultiplier { get; set; }
        public int Amount { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
    }

    public class Bonuses
    {
        public int Host { get; set; }
        public int Cheer { get; set; }
        public int Subscriber { get; set; }
        public int Tip { get; set; }
        public int Follow { get; set; }
    }
}