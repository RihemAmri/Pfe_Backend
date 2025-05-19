namespace Statistiques.API.DTO
{
    public class TotauxDto
    {
        public int TotalUsers { get; set; }
        public int TotalDemandes { get; set; }
        public int TotalCredits { get; set; }


        public int TotalDemandesAcceptees { get; set; }
        public int TotalDemandesRefusees { get; set; }
        public int TotalCreditsEnCours { get; set; }
        public int TotalDemandesEnAttente { get; set; }
    }
}
