//using System.Data;
//using System.Threading.Tasks;

//namespace Data.Helpers
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class MySqlDbStore
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        private readonly MySqlConnection _connection;

//        /// <summary>
//        /// 
//        /// </summary>
//        private readonly string _connectionString;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="settings"></param>
//        public MySqlDbStore(DbSettings settings)
//        {
//            _connectionString = settings.Uri;
//            _connection = new MySqlConnection(_connectionString);
//        }

//        public string GetConnectionString()
//        {
//            return _connectionString;
//        }

//        /// <summary>
//        /// Get ForumUser from MySQL
//        /// </summary>
//        /// <param name="emailUsername">The User's Email or Username</param>
//        /// <returns></returns>
//        public virtual async Task<User> GetUserAsync(string emailUsername)
//        {
//            emailUsername = MySqlHelper.EscapeString(emailUsername);
//            string sqlWhere;

//            if (emailUsername.Contains("@") && emailUsername.Contains("."))
//            {
//                sqlWhere = "WHERE email = @EmailUsername";
//            }
//            else
//            {
//                sqlWhere = "WHERE name = @EmailUsername";
//            }

//            var command = new MySqlCommand()
//            {
//                CommandText = $"SELECT member_id, name, email, member_group_id, members_pass_salt, members_pass_hash, pp_thumb_photo, ip_address FROM dogcore_members {sqlWhere}",
//                Connection = _connection,
//                CommandType = CommandType.Text
//            };

//            command.Parameters.AddWithValue("@EmailUsername", emailUsername);


//            await _connection.OpenAsync();
//            var reader = command.ExecuteReader();
//            var result = new User();
//            while (reader.Read())
//            {
//                result.ForumId = int.Parse(reader[0].ToString());
//                result.DisplayName = reader[1].ToString();
//                result.Email = reader[2].ToString();
//                result.AccountType = (AccountType)int.Parse(reader[3].ToString());
//                result.PassHash = reader[5].ToString();
//                result.PassSalt = reader[4].ToString();
//                result.ProfilePictureUrl = reader[6].ToString();
//                result.IpAddress = reader[7].ToString();
//            }

//            await _connection.CloseAsync();
//            return result;
//        }
//    }
//}
