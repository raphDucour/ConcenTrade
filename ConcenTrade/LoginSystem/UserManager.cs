using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Xml.Linq;
using Supabase;
using System.Threading.Tasks;
using System;
using System.Linq;
using Supabase.Gotrue;
using System.Windows;

namespace Concentrade
{
    public static class UserManager
    {
        private static string SupabaseUrl = "https://vmxaqzggcidruxgvctrs.supabase.co";
        private static string SupabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InZteGFxemdnY2lkcnV4Z3ZjdHJzIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTAyNDM0NDQsImV4cCI6MjA2NTgxOTQ0NH0.z7EC8JwvcKTwr2fbGsGCRpuMqziMXStE9wkL-YTXQUk";

        private static Supabase.Client _supabase;

        /// <summary>Initialise le client Supabase</summary>
        static UserManager()
        {
            _supabase = new Supabase.Client(SupabaseUrl, SupabaseKey);
        }

        /// <summary>Enregistre un nouvel utilisateur avec email et mot de passe</summary>
        public static async Task<bool> Register(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignUp(email, password);

                if (session== null || session.User == null)
                {
                    MessageBox.Show("Activez votre connexion internet svp!");
                    return false;
                }

                var newUserProfile = new User();
                newUserProfile.email = email;
                newUserProfile.QuestionnaireDone = false;
                var insertResult = await _supabase.From<User>().Insert(newUserProfile);
                if (insertResult.Models.Count == 0)
                {
                    MessageBox.Show("Erreur lors de la création du profil utilisateur (insertion échouée).");
                    return false;
                }

                await UserManager.LoadProperties(email);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetSignUpFriendlyErrorMessage(ex.Message));
                return false;
            }
        }

        /// <summary>Met à jour les propriétés utilisateur dans la base de données</summary>
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

        /// <summary>Lance PushIntoBDD de manière asynchrone sans attendre le résultat</summary>
        public static void PushIntoBDD_FireAndForget()
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await PushIntoBDD();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur dans PushIntoBDD : " + ex.Message);
                }
            });
        }

        /// <summary>Recherche un utilisateur par son email</summary>
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

        /// <summary>Authentifie un utilisateur avec email et mot de passe</summary>
        public static async Task<bool> Login(string email, string password)
        {
            try
            {
                var session = await _supabase.Auth.SignIn(email, password);

                if (session == null || session.User == null)
                {
                    System.Windows.MessageBox.Show("Activez votre connexion internet svp!");
                    return false;
                }

                await LoadProperties(email);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(GetLoginFriendlyErrorMessage(ex.Message));
                return false;
            }
        }

        /// <summary>Charge les propriétés utilisateur depuis la base de données</summary>
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
        public static void LoadProperties_FireAndForget(string email)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await LoadProperties(email);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur dans LoadProperiesFireAndForget : " + ex.Message);
                }
            });
        }


        /// <summary>Sauvegarde les points d'un utilisateur dans la base de données</summary>
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

        /// <summary>Convertit les erreurs d'inscription en messages utilisateur compréhensibles</summary>
        public static string GetSignUpFriendlyErrorMessage(string errorMessage)
        {
            if (errorMessage.Contains("user already exists", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("email_exists", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("already registered", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("user_already_exists", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("email address already exists", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("email already in use", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("email already exists", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("duplicate key value violates unique constraint", StringComparison.OrdinalIgnoreCase))
            {
                return "Cet email est déjà utilisé. Veuillez en choisir un autre ou vous connecter.";
            }
            if (errorMessage.Contains("weak password", StringComparison.OrdinalIgnoreCase) ||
                (errorMessage.Contains("password", StringComparison.OrdinalIgnoreCase) && errorMessage.Contains("weak", StringComparison.OrdinalIgnoreCase)))
            {
                return "Le mot de passe est trop faible. Choisissez un mot de passe plus complexe (au moins 8 caractères, avec chiffres et lettres).";
            }
            if (errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("too many requests", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("429", StringComparison.OrdinalIgnoreCase))
            {
                return "Trop de tentatives. Veuillez patienter quelques instants avant de réessayer.";
            }
            if (errorMessage.Contains("network", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("No such host", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("Aucune connexion", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("network unreachable", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("impossible de contacter le serveur", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("hôte inconnu", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("unknown host", StringComparison.OrdinalIgnoreCase))
            {
                return "Problème de connexion au serveur. Vérifiez votre connexion internet.";
            }
            string error = $"Une erreur est survenue. Merci de réessayer ou de contacter le support si le problème persiste : {errorMessage}";
            return error;
        }

        /// <summary>Convertit les erreurs de connexion en messages utilisateur compréhensibles</summary>
        public static string GetLoginFriendlyErrorMessage(string errorMessage)
        {
            if (errorMessage.Contains("invalid login credentials", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("invalid credentials", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("identifiants incorrects", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("wrong password", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("mot de passe incorrect", StringComparison.OrdinalIgnoreCase))
            {
                return "Email ou mot de passe incorrect.";
            }
            if (errorMessage.Contains("email not confirmed", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("email_not_confirmed", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("unconfirmed", StringComparison.OrdinalIgnoreCase))
            {
                return "Votre email n'a pas été confirmé. Veuillez vérifier votre boîte mail.";
            }
            if (errorMessage.Contains("user not found", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("user_not_found", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("aucun utilisateur", StringComparison.OrdinalIgnoreCase))
            {
                return "Aucun compte n'est associé à cet email.";
            }
            if (errorMessage.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("too many requests", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("429", StringComparison.OrdinalIgnoreCase))
            {
                return "Trop de tentatives. Veuillez patienter quelques instants avant de réessayer.";
            }
            if (errorMessage.Contains("network", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("No such host", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("Aucune connexion", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("network unreachable", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("impossible de contacter le serveur", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("hôte inconnu", StringComparison.OrdinalIgnoreCase) ||
                errorMessage.Contains("unknown host", StringComparison.OrdinalIgnoreCase))
            {
                return "Problème de connexion au serveur. Vérifiez votre connexion internet.";
            }
            string error = $"Une erreur est survenue. Merci de réessayer ou de contacter le support si le problème persiste : {errorMessage}";
            return error;
        }
    }
}
