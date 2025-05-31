using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace Concentrade
{
    public static class UserManager
    {
        private static readonly BDD.BDDHandler db = new BDD.BDDHandler();

        public static bool Register(string email, string password)
        {
            // Vérifie si l'utilisateur existe déjà
            string checkQuery = "SELECT COUNT(*) FROM Users WHERE email = @Email";
            var exists = Convert.ToInt32(db.ExecuteScalar(checkQuery, new MySqlParameter("@Email", email))) > 0;
            if (exists) return false;

            string hashedPassword = User.HashPassword(password);
            string insertQuery = @"
                INSERT INTO Users (email, username, password)
                VALUES (@Email, @Username, @Password)";
            db.ExecuteNonQuery(insertQuery,
                new MySqlParameter("@Email", email),
                new MySqlParameter("@Password", hashedPassword));
            return true;     
        }

        public static User? Login(string email, string password)
        {
            string hashed = User.HashPassword(password);
            string query = "SELECT * FROM Users WHERE email = @Email AND password = @Password";
            DataTable result = db.ExecuteQuery(query,
                new MySqlParameter("@Email", email),
                new MySqlParameter("@Password", hashed));

            if (result.Rows.Count == 0) return null;

            var row = result.Rows[0];
            return new User(email, hashed)
            {
                //Name = row["username"]?.ToString(),
                Age = row["age"] == DBNull.Value ? 0 : Convert.ToInt32(row["age"]),
                //BestMoment = row["best_moment"]?.ToString(),
                Distraction = Convert.ToBoolean(row["distraction"]),
                QuestionnaireDone = Convert.ToBoolean(row["questionnaire_done"]),
                // La propriété LaunchOnStartup n'existe pas dans la BDD actuelle
            };
        }

        public static void SetUserProfile(string email, string username, int age, string bestMoment, bool distraction)
        {
            string query = @"
                UPDATE Users SET 
                    username = @Username,
                    age = @Age,
                    best_moment = @BestMoment,
                    distraction = @Distraction,
                    questionnaire_done = 1
                WHERE email = @Email";

            db.ExecuteNonQuery(query,
                new MySqlParameter("@Username", username),
                new MySqlParameter("@Age", age),
                new MySqlParameter("@BestMoment", bestMoment),
                new MySqlParameter("@Distraction", distraction),
                new MySqlParameter("@Email", email));
        }

        public static User? FindUser(string email)
        {
            string query = "SELECT * FROM Users WHERE email = @Email";
            DataTable result = db.ExecuteQuery(query, new MySqlParameter("@Email", email));

            if (result.Rows.Count == 0) return null;
            var row = result.Rows[0];

            return new User(email,"")// row["password"].ToString())
            {
                //Name = row["username"]?.ToString(),
                Age = row["age"] == DBNull.Value ? 0 : Convert.ToInt32(row["age"]),
                //BestMoment = row["best_moment"]?.ToString(),
                Distraction = Convert.ToBoolean(row["distraction"]),
                QuestionnaireDone = Convert.ToBoolean(row["questionnaire_done"]),
                // Toujours pas de LaunchOnStartup dans cette BDD
            };
        }

        public static void LoadProperties(string email)
        {
            var user = FindUser(email);
            if (user == null) return;

            Properties.Settings.Default.UserEmail = email;
            Properties.Settings.Default.UserName = user.Name;
            Properties.Settings.Default.UserAge = user.Age;
            Properties.Settings.Default.BestMoment = user.BestMoment;
            Properties.Settings.Default.Distraction = user.Distraction;
            Properties.Settings.Default.QuestionnaireDone = user.QuestionnaireDone;
            Properties.Settings.Default.Save();
        }
    }
}
