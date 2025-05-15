// DTO pour la réponse à une anticipation
public class ReponseAnticipationDto
{
    public string IdDemande { get; set; }
    public string ReponseAdmin { get; set; } // Exemple : "Acceptée" ou "Refusée"
    public string Commentaire { get; set; }
    public DateTime DateReponse { get; set; } = DateTime.UtcNow;
}
