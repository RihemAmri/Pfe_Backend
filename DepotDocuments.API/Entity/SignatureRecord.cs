using DepotDocuments.API.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepotDocuments.API.Entities

{
   public class SignatureRecord
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string DemandeId { get; set; } // ID de la demande liée

    public string SignatureType { get; set; } // "drawn" ou "uploaded"

    public string CloudinaryUrl { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
}
}