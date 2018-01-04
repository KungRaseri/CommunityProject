using System.Collections.Generic;

namespace Data.Models.StreamElements.Giveaways
{
    public class GetAllGiveawaysResponse
    {
        public Giveaway Active { get; set; }
        public IEnumerable<Giveaway> Giveaways { get; set; }
    }
}
