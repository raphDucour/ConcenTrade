using System.Text;
using System.Security.Cryptography;
using Supabase.Postgrest.Attributes;
using System;

namespace Concentrade
{
    // Indique le nom de la table dans Supabase.
    [Table("User")]
    public class User : Supabase.Postgrest.Models.BaseModel // Héritez de BaseModel
    {
        // Nouvelle clé primaire Id générée par la base de données (auth.uid())
        [PrimaryKey("Id", false)] // "Id" est la clé primaire, "false" indique qu'elle n'est pas générée par le client
        public Guid Id { get; set; }

        [Column("Email")]
        public string? email { get; set; }

        // Mappe PasswordHash à la colonne "Password" de la DB
        [Column("Password")]
        public string? PasswordHash { get; set; }

        [Column("Name")]
        public string Name { get; set; } = "";

        [Column("BestMoment")]
        public string BestMoment { get; set; } = "";
        [Column("Distraction")]
        public bool Distraction { get; set; } = false;
        [Column("LaunchOnStartup")]
        public bool LaunchOnStartup { get; set; } = false;
        [Column("QuestionnaireDone")]
        public bool QuestionnaireDone { get; set; } = false;

        [Column("BlockedApps")]
        public string BlockedApps { get; set; } = "";

        // Renommé de IgnoredDefaultApps à IgnoredApps pour correspondre au schéma
        [Column("IgnoredApps")]
        public string IgnoredApps { get; set; } = "";

        // Nouvelle propriété "Cards"
        [Column("Cards")]
        public string Cards { get; set; } = "";

        // Ajout de la propriété BirthDate
        [Column("BirthDate")]
        public DateTime? BirthDate { get; set; } // Utilisation de DateTime? pour permettre les valeurs null

        // Réajout de la propriété Points
        [Column("Points")]
        public long Points { get; set; } = 0; // Utilisation de long pour correspondre à bigint dans PostgreSQL

        // Le constructeur doit être ajusté car l'Id est généré par la DB
        public User(string Email, string passwordHash)
        {
            email = Email;
            PasswordHash = passwordHash;
            // Id ne doit pas être défini ici car il est généré par la DB
        }

        // Requis par le désérialiseur JSON / Supabase
        public User() { }

        public static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Ajustez cette méthode en fonction des champs restants
        public void RemplirDepuisQuestionnaire(string prenom, string moment, bool launchOnStratup, bool distrait)
        {
            Name = prenom;
            BestMoment = moment;
            Distraction = distrait;
            LaunchOnStartup = launchOnStratup;
            QuestionnaireDone = true;
            // BirthDate n'est pas rempli ici, supposant qu'il est défini ailleurs si nécessaire
            // Points n'est pas rempli ici, supposant qu'il est géré séparément
        }
    }
}