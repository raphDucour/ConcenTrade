using System.Text;
using System.Security.Cryptography;

namespace Concentrade
{
    public class User
    {
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }

        public User(string email, string username, string passwordHash)
        {
            Email = email;
            Username = username;
            PasswordHash = passwordHash;
        }

        // Requis par le désérialiseur JSON
        public User() { }

        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
