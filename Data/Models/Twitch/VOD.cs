using System;
using Data.Interfaces;
using TwitchLib.Api.Models.v5.Videos;

namespace Data.Models.Twitch
{
    public class VOD : IEntity
    {
        public string Id { get; set; }
        public string Rev { get; set; }
        public DateTime ImportedAt { get; set; }
        public Video Video { get; set; }
    }
}
