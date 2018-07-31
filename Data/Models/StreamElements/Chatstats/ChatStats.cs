using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.StreamElements.Chatstats
{
    public class ChatStats
    {
        public string Channel { get; set; }
        public int TotalMessages { get; set; }
        public IEnumerable<ChatterStat> Chatters { get; set; }
        public IEnumerable<HashTagStat> HashTags { get; set; }
        public IEnumerable<CommandStat> Commands { get; set; }
        public IEnumerable<BTTVEmoteStat> BttvEmotes { get; set; }
        public IEnumerable<TwitchEmoteStat> TwitchEmotes { get; set; }
    }

    public class BTTVEmoteStat
    {
        public string Id { get; set; }
        public string Emote { get; set; }
        public int Amount { get; set; }
    }

    public class TwitchEmoteStat
    {
        public string Id { get; set; }
        public string Emote { get; set; }
        public int Amount { get; set; }
    }

    public class CommandStat
    {
        public string Command { get; set; }
        public int Amount { get; set; }
    }

    public class HashTagStat
    {
        public string HashTag { get; set; }
        public int Amount { get; set; }
    }

    public class ChatterStat
    {
        public string Name { get; set; }
        public int Amount { get; set; }
    }

    public class ChatStatSettings
    {
        public IEnumerable<string> IgnoredChatters { get; set; }
    }
}
