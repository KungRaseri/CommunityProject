using System.Collections.Generic;
using Data.Interfaces;

namespace Data.Models.CouchDB
{
    public class DesignDocument
    {
        public Dictionary<string, View> views { get; set; }
        public string language { get; set; }
    }
}
