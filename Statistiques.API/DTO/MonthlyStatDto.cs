namespace Statistiques.API.DTO
{
   public class MonthlyStatDto
    {
        public string Mois { get; set; }  // Exemple : "Mars", "Avril"
        public int Total { get; set; }    // Nombre de demandes ou Montant total
    }
}