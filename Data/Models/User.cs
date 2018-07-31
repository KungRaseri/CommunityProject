using Data.Interfaces;

namespace Data.Models
{
    public class User : IEntity
    {
        public string _id { get; set; }
        public string _rev { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
    }
}
