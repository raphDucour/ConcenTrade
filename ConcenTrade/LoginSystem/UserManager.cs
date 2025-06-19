using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Concentrade
{
    public static class UserManager
    {
        private static string filePath = "users.json";

        public static List<User> LoadUsers()
        {
            if (!File.Exists(filePath)) return new List<User>();
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void SaveBlockedAppsForUser(string email, IEnumerable<string> blockedApps)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);
            if (user != null)
            {
                user.BlockedApps = string.Join(",", blockedApps); // transforme la liste en string séparée par des virgules
                SaveUsers(users);
            }
        }


        public static string LoadBlockedAppsForUser(string email)
        {
            var user = FindUser(email);
            // Retourne la liste de l'utilisateur, ou une nouvelle liste vide si l'utilisateur ou la liste n'existe pas.
            string BlockedApps= user.BlockedApps;
            return BlockedApps;
        }

        public static void SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static bool Register(string email, string password)
        {
            var users = LoadUsers();
            if (users.Exists(u => u.Email == email)) return false;

            users.Add(new User(email,User.HashPassword(password)));
            SaveUsers(users);
            return true;
        }

        // Méthode pour définir le profil
        public static void SetUserProfile(string email, string name, int age, DateTime birthDate, string bestMoment, bool distraction, bool launchOnStartup = false)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);

            if (user != null)
            {
                user.Name = name;
                user.Age = age;
                user.UserBirthDate = birthDate; // Ligne ajoutée
                user.BestMoment = bestMoment;
                user.Distraction = distraction;
                user.LaunchOnStartup = launchOnStartup;
                user.QuestionnaireDone = true;
                SaveUsers(users);
            }
        }

        public static User? FindUser(string email)
        {
            var users = LoadUsers();
            return users.Find(u => u.Email == email);
        }

        public static User? Login(string email, string password)
        {
            var users = LoadUsers();
            string hash = User.HashPassword(password);
            return users.Find(u => u.Email == email && u.PasswordHash == hash);
        }

        // Méthode pour charger les propriétés
        public static void LoadProperties(string email)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);

            if (user != null)
            {
                Properties.Settings.Default.UserEmail = email;
                Properties.Settings.Default.UserName = user.Name;
                Properties.Settings.Default.UserAge = user.Age;
                Properties.Settings.Default.UserBirthDate = user.UserBirthDate; // Ligne ajoutée
                Properties.Settings.Default.BestMoment = user.BestMoment;
                Properties.Settings.Default.Distraction = user.Distraction;
                Properties.Settings.Default.LaunchOnStartup = user.LaunchOnStartup;
                Properties.Settings.Default.QuestionnaireDone = user.QuestionnaireDone;
                Properties.Settings.Default.Points = user.Points;
                Properties.Settings.Default.BlockedApps = user.BlockedApps;

                Properties.Settings.Default.Save();
            }
        }

        public static List<string> LoadIgnoredAppsForUser(string email)
        {
            var user = FindUser(email);
            // On split le string séparé par des virgules en liste
            if (user == null || string.IsNullOrWhiteSpace(user.IgnoredDefaultApps))
                return new List<string>();
            return user.IgnoredDefaultApps
                       .Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(app => app.Trim())
                       .ToList();
        }

        public static void SaveIgnoredAppsForUser(string email, IEnumerable<string> ignoredApps)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);
            if (user != null)
            {
                // On join la liste en string séparé par des virgules
                user.IgnoredDefaultApps = string.Join(",", ignoredApps.Select(app => app.Trim()));
                SaveUsers(users);
            }
        }

        public static void SavePoints(string email, int points)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);

            if (user != null)
            {
                user.Points = points;
                SaveUsers(users);
            }
        }
    }
}
