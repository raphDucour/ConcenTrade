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
    }
}
