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
        public bool Distraction { get; set; } = false;
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
        public void RemplirDepuisQuestionnaire(string prenom, int age, string moment, bool launchOnStratup, bool distrait)
        {
            Name = prenom;
            Age = age;
            BestMoment = moment;
            Distraction = distrait;
            LaunchOnStartup = launchOnStratup;
            QuestionnaireDone = true;
        }
    }
}
