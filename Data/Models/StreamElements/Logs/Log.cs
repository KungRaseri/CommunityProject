using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.StreamElements.Logs
{
    public class Log
    {
        public string _id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IP { get; set; }
        public string Source { get; set; }
        public string Event { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
    }
}
