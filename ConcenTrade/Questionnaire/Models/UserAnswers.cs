namespace Concentrade
{
    public class UserAnswers
    {
        public string Prenom { get; set; } = "";
        public string Age { get; set; } = "";
        public string Sexe { get; set; } = "";
        public string Moment { get; set; } = "";
        public string Distrait { get; set; } = "";

        public override string ToString()
        {
            return $"Prénom : {Prenom}\n" +
                   $"Âge : {Age}\n" +
                   $"Sexe : {Sexe}\n" +
                   $"Moment : {Moment}\n" +
                   $"Distrait : {Distrait}";
        }

        public void SauvegarderDansSettings()
        {
            Properties.Settings.Default.UserName = Prenom;

            // Essayons de convertir l'âge en entier si possible
            if (int.TryParse(Age, out int ageInt))
                Properties.Settings.Default.UserAge = ageInt;
            else
                Properties.Settings.Default.UserAge = 0;

            Properties.Settings.Default.BestMoment = Moment;

            // On considère "oui" ou "true" comme vrai pour "Distrait"
            Properties.Settings.Default.Distraction = Distrait.ToLower() == "oui" || Distrait.ToLower() == "un petit peu";

            Properties.Settings.Default.QuestionnaireDone = true;
            // Enregistre les modifications
            Properties.Settings.Default.Save();
        }
    }
}
