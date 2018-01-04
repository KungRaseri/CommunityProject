using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.StreamElements.Booties
{
    public class GetTopBootiesResponse
    {
        public int _total { get; set; }
        public UserRankResponse[] Users { get; set; }
    }
}
