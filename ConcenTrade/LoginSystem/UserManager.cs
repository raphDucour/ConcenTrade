using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Xml.Linq;
using Supabase;
using System.Threading.Tasks;
using System;
using System.Linq;
using Supabase.Gotrue; // <--- ASSUREZ-VOUS QUE CETTE LIGNE EST TOUJOURS PRÉSENTE !
using System.Windows;

namespace Concentrade
{
    public static class UserManager
    {
        // Vos identifiants Supabase
        private static string SupabaseUrl = "https://vmxaqzggcidruxgvctrs.supabase.co";
        private static string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZteGFxemdnY2lkcnV4Z3ZjdHJzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTAyNDM0NDQsImV4cCI6MjA2NTgxOTQ0NH0.z7EC8JwvcKTwr2fbGsGCRpuMqziMXStE9wkL-YTXQUk";

        private static Supabase.Client _supabase;

        static UserManager()
        {
            // Initialisation du client Supabase
            _supabase = new Supabase.Client(SupabaseUrl, SupabaseKey);
        }

        public static async Task<bool> Register(string email, string password)
        {
            try
            {
                var authResponse = await _supabase.Auth.SignUp(email, password);

                if (authResponse.User == null)
                {
                    string errorMessage = "Une erreur inconnue est survenue lors de l'inscription.";
                    Console.WriteLine(errorMessage);
                    return false;
                }

                // Création du profil utilisateur dans la table User avec email comme clé primaire
                var newUserProfile = new User();
                newUserProfile.email = email;
                newUserProfile.QuestionnaireDone = false;
                var insertResult = await _supabase.From<User>().Insert(newUserProfile);
                if (insertResult.Models.Count == 0)
                {
                    System.Windows.MessageBox.Show("Erreur lors de la création du profil utilisateur (insertion échouée).");
                    return false;
                }

                await UserManager.LoadProperties(email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'enregistrement ou de la création du profil utilisateur: {ex.Message}");
                return false;
            }
        }

        public static async Task PushIntoBDD()
        {
            try
            {
                string userEmail = Properties.Settings.Default.UserEmail;

                var result = await _supabase.From<User>().Where(u => u.email == userEmail).Get();
                var user = result.Models.FirstOrDefault();

                if (user != null)
                {
                    user.Name = Properties.Settings.Default.UserName;
                    user.BestMoment = Properties.Settings.Default.BestMoment;
                    user.Distraction = Properties.Settings.Default.Distraction;
                    user.LaunchOnStartup = Properties.Settings.Default.LaunchOnStartup;
                    user.QuestionnaireDone = Properties.Settings.Default.QuestionnaireDone;
                    user.BlockedApps = Properties.Settings.Default.BlockedApps;
                    user.IgnoredApps = Properties.Settings.Default.IgnoredApps;
                    user.Cards = Properties.Settings.Default.Cards;
                    user.BirthDate = Properties.Settings.Default.UserBirthDate;
                    user.Points = Properties.Settings.Default.Points;

                    await _supabase.From<User>().Update(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour des propriétés de l'utilisateur : {ex.Message}");
            }
        }

        public static async Task<User?> FindUser(string email)
        {
            try
            {
                var result = await _supabase.From<User>().Where(u => u.email == email).Get();
                return result.Models.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche de l'utilisateur : {ex.Message}");
                return null;
            }
        }

        public static async Task<bool> Login(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignIn(email, password);

                if (session == null || session.User == null)
                {
                    System.Windows.MessageBox.Show("Identifiants incorrects ou email non confirmé.");
                    return false;
                }

                await LoadProperties(email); // Optionnel : charge les propriétés si besoin
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erreur lors de la connexion : " + ex.Message);
                return false;
            }
        }

        public static async Task LoadProperties(string email)
        {
            try
            {
                var user = await FindUser(email);

                if (user != null)
                {
                    Properties.Settings.Default.UserEmail = email;
                    Properties.Settings.Default.UserName = user.Name;
                    Properties.Settings.Default.BestMoment = user.BestMoment;
                    Properties.Settings.Default.Distraction = user.Distraction;
                    Properties.Settings.Default.LaunchOnStartup = user.LaunchOnStartup;
                    Properties.Settings.Default.QuestionnaireDone = user.QuestionnaireDone;
                    Properties.Settings.Default.BlockedApps = user.BlockedApps;
                    Properties.Settings.Default.IgnoredApps = user.IgnoredApps;
                    Properties.Settings.Default.Cards = user.Cards;
                    Properties.Settings.Default.UserBirthDate = user.BirthDate ?? DateTime.MinValue;
                    Properties.Settings.Default.Points = (int)user.Points;

                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des propriétés : {ex.Message}");
            }
        }

        public static async Task SavePoints(string email, long points)
        {
            try
            {
                var result = await _supabase.From<User>().Where(u => u.email == email).Get();
                var user = result.Models.FirstOrDefault();

                if (user != null)
                {
                    user.Points = points;
                    await _supabase.From<User>().Update(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde des points : {ex.Message}");
            }
        }
    }
}