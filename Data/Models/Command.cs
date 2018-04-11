﻿using Data.Interfaces;

namespace Data.Models
{
    public class Command : IEntity
    {
        public string Name { get; set; }
        public AuthLevel AuthorizationLevel { get; set; }
        public string Instructions { get; set; }
        public string Identifier { get; set; }
        public bool IsActive { get; set; }

        public string Id { get; set; }
        public string Rev { get; set; }
    }
}