using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DepotDocuments.API.Entities
{
public class DepotDemande
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string UserId { get; set; }

    // Infos utilisateur
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public string Cin { get; set; }

    public string DateNaissance { get; set; }
    public string Telephone { get; set; }
    public string Civilite { get; set; }

    // Simulation
    public decimal MontantDemande { get; set; }
    public decimal MensualiteEstimee { get; set; }
    public int DureeEnAnnees { get; set; }
    public string TypeCredit { get; set; }
    public string TypeFinancement { get; set; }

    // OCR
    public string RevenuMensuelOcr { get; set; }
    public string AttestationSalaireOcr { get; set; }

    // Liste des IDs de documents liés
    public List<string> DocumentIds { get; set; }
    
    public string Statut { get; set; } = "soumise";
    // Date de soumission
    public DateTime DateDepot { get; set; } = DateTime.UtcNow;
}
}