using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Declarations.API.Entities
{
    public class Reponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id_reponse")]
        public string id_reponse { get; set; }

        public string DeclarationId { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
