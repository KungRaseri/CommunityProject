using System.Collections.Generic;

namespace Data.Models.StreamElements.Giveaways
{
    public class GetPastGiveawaysResponse
    {
        public IEnumerable<Giveaway> Giveaways { get; set; }
    }
}