using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepotDocuments.API.Entities
{
    public class CreditDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string FileType { get; set; } // cin, fiche_paie, attestation_salaire

        public string FileUrl { get; set; }

        public string ExtractedText { get; set; }
    }
}
