using Data.Interfaces;

namespace Data.Models
{
    public class Settings : IEntity
    {
        public string StreamElementsAPIUrl => "https://api.streamelements.com/kappa/v2";
        public string Id { get; set; }
        public string Rev { get; set; }
        public string CommandCharacter { get; set; }
        public Keys Keys { get; set; }
        public string CouchDbUri { get; set; }
        public string MySqlUri { get; set; }
    }
}
