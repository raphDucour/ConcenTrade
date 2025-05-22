using System.Text;
using System.Security.Cryptography;

namespace Concentrade
{
    public class User
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        // Nouveaux champs personnalisés
        public string Name { get; set; } = "";
        public int Age { get; set; } = 0;
        public string BestMoment { get; set; } = "";
        public string Distraction { get; set; } = "";
        public bool LaunchOnStartup { get; set; } = false;
        public bool QuestionnaireDone { get; set; } = false;
        

        public User(string email, string passwordHash)
        {
            Email = email;
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
