using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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

        public static void SetUserProfile(string email, string name, int age, string bestMoment, bool distraction, bool launchOnStartup = false)
        {
            var users = LoadUsers();
            var user = users.Find(u => u.Email == email);

            if (user != null)
            {
                user.Name = name;
                user.Age = age;
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
    }
}
