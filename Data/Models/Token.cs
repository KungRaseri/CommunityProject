using System;
using Data.Interfaces;

namespace Data.Models
{
    public class Token : IEntity
    {
        public string Id { get; set; }
        public string Rev { get; set; }

        public string Value { get; set; }
        public string UserId { get; set; }
        public string TwitchUsername { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime Issued { get; set; }
    }
}