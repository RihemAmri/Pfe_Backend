using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Credit.API.Models
{
    public class DemandeAnticipation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdDemande { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdCredit { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string IdClient { get; set; }
        public string NumeroCompte { get; set; }
        public string Raison { get; set; }

        public string Statut { get; set; } = "En Attente"; // EnAttente, Approuvee, Refusee

        public string? ReponseAdmin { get; set; }
        public string? Commentaire { get; set; }
        public DateTime DateDemande { get; set; } = DateTime.UtcNow;
        public DateTime? DateReponse { get; set; }
        public string? AttestationPaiementUrl { get; set; }  // ➜ URL du PDF sur Cloudinary

    }
}
