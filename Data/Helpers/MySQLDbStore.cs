using Data.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Data.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlDbStore
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly MySqlConnection _connection;

        /// <summary>
        /// 
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public MySqlDbStore(ApplicationSettings settings)
        {
            _connectionString = ApplicationSettings.MySqlUrl;
            _connection = new MySqlConnection(_connectionString);
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// Get ForumUser from MySQL
        /// </summary>
        /// <param name="emailUsername">The User's Email or Username</param>
        /// <returns></returns>
        public virtual async Task<Account> GetUserAsync(string emailUsername)
        {
            emailUsername = MySqlHelper.EscapeString(emailUsername);
            string sqlWhere;

            if (emailUsername.Contains("@") && emailUsername.Contains("."))
            {
                sqlWhere = "WHERE email = @EmailUsername";
            }
            else
            {
                sqlWhere = "WHERE name = @EmailUsername";
            }

            var command = new MySqlCommand()
            {
                CommandText = $"SELECT id, name, email, password, salt FROM users {sqlWhere}",
                Connection = _connection,
                CommandType = CommandType.Text
            };

            command.Parameters.AddWithValue("@EmailUsername", emailUsername);


            await _connection.OpenAsync();
            var reader = command.ExecuteReader();
            var result = new Account();
            while (reader.Read())
            {
                result._id = reader[0].ToString();
                result.Email = reader[1].ToString();
                result.Password = reader[5].ToString();
                result.PasswordSalt = reader[4].ToString();
            }

            await _connection.CloseAsync();
            return result;
        }
    }
}
