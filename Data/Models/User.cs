using Data.Interfaces;

namespace Data.Models
{
    public class User : IEntity
    {
        public string Id { get; set; }
        public string Rev { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string TwitchUsername { get; set; }
        public bool IsFollower { get; set; }
        public bool IsSubscriber { get; set; }
    }
}
