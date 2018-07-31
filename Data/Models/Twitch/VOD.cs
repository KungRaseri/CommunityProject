using System;
using Data.Interfaces;
using TwitchLib.Api.Models.v5.Videos;

namespace Data.Models.Twitch
{
    public class Vod : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public DateTime ImportedAt { get; set; }
        public Video Video { get; set; }
    }
}
