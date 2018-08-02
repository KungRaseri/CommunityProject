using System;
using System.Collections.Generic;
using System.Text;
using Data.Interfaces;

namespace Data.Models
{
    public class ViewerRank : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string RankName { get; set; }
        public int ExperienceRequired { get; set; }
    }
}
