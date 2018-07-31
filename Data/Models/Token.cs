using System;
using Data.Interfaces;

namespace Data.Models
{
    public class Token : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }

        public string Value { get; set; }
        public string UserId { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime Issued { get; set; }
    }
}