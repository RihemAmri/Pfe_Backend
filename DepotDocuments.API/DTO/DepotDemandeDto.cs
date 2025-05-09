namespace DepotDocuments.API.DTO
{
public class DepotDemandeDto
{
    public string UserId { get; set; }

    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string Cin { get; set; }

     public string NumeroCompte { get; set; }
    public string DateNaissance { get; set; }
    public string Telephone { get; set; }
    public string Civilite { get; set; }

    public decimal MontantDemande { get; set; }
    public decimal MensualiteEstimee { get; set; }
    public int DureeEnAnnees { get; set; }
    public string TypeCredit { get; set; }
    public string TypeFinancement { get; set; }

    public string RevenuMensuelOcr { get; set; }
    public string AttestationSalaireOcr { get; set; }

    public List<string> DocumentIds { get; set; }
    public DateTime? DateDerniereModification { get; set; }
    public string? PdfBase64 { get; set; }
}
}