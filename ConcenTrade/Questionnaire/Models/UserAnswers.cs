using System;
using System.Windows;

namespace Concentrade
{
    public class UserAnswers
    {
        public string Prenom { get; set; } = "";
        private DateTime _dateNaissance;
        public string DateNaissance
        {
            get => _dateNaissance.ToString("dd/MM/yyyy");
            set => _dateNaissance = DateTime.ParseExact(value, "dd/MM/yyyy", null);
        }
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - _dateNaissance.Year;
                if (_dateNaissance.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
        public string Sexe { get; set; } = "";
        public string Moment { get; set; } = "";
        public string Distrait { get; set; } = "";
        public int DureeMinutes { get; set; } = 0;


        public override string ToString()
        {
            return $"Prénom : {Prenom}\n" +
                   $"Date de naissance : {DateNaissance}\n" +
                   $"Âge : {Age} ans\n" +
                   $"Sexe : {Sexe}\n" +
                   $"Moment : {Moment}\n" +
                   $"Distrait : {Distrait}";
        }



        public void SauvegarderDansSettings()
        {
            Properties.Settings.Default.UserName = Prenom;
            Properties.Settings.Default.UserAge = Age;
            Properties.Settings.Default.BestMoment = Moment;
            Properties.Settings.Default.Distraction = Distrait.ToLower() == "oui" || Distrait.ToLower() == "un petit peu";
            Properties.Settings.Default.QuestionnaireDone = true;
            Properties.Settings.Default.Save();
        }


        public void SauvegarderDansLaBaseDeDonnees()
        {
            string email = Properties.Settings.Default.UserEmail;

            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Erreur : aucun email enregistré dans les paramètres.");
                return;
            }
            else
            {
                // Appel à UserManager pour mise à jour dans le JSON
                UserManager.SetUserProfile(
                    email,
                    Prenom,
                    Age,
                    Moment,
                    Distrait.ToLower() == "oui" || Distrait.ToLower() == "un petit peu",
                    false
                );
            }
        }
    }
}
