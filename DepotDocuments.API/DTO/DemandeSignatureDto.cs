namespace DepotDocuments.API.DTO
{
public class DemandeSignatureDto
{
    // SECTION 1 - Infos personnelles
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string DateNaissance { get; set; }
    public string Cin { get; set; }
    public string NumeroCompte { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string Civilite { get; set; }

    // SECTION 2 - Situation financière
    public string RevenusMensuels { get; set; }
    public string RevenusAnnuels { get; set; }

    // SECTION 3 - Données de la demande
    public string TypeCredit { get; set; }
    public string TypeFinancement { get; set; }
    public string MontantDemande { get; set; }
    public string Duree { get; set; }
    public string MensualiteEstimee { get; set; }
    
}
}