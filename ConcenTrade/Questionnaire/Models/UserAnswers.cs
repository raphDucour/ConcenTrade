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

        public string Moment { get; set; } = "";
        public string Distrait { get; set; } = "";
        public int DureeMinutes { get; set; } = 0;


        public override string ToString()
        {
            return $"Prénom : {Prenom}\n" +
                   $"Date de naissance : {DateNaissance}\n" +
                   $"Moment : {Moment}\n" +
                   $"Distrait : {Distrait}";
        }



        public void SauvegarderDansSettings()
        {
            Properties.Settings.Default.UserName = Prenom;
            Properties.Settings.Default.UserBirthDate = _dateNaissance;
            Properties.Settings.Default.BestMoment = Moment;
            Properties.Settings.Default.Distraction = Distrait.ToLower() == "oui" || Distrait.ToLower() == "un petit peu";
            Properties.Settings.Default.QuestionnaireDone = true;
            Properties.Settings.Default.Save();
        }
    }
}
