// Declaration.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Declarations.API.Entities
{
    public class Declaration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id_declaration")]
        public string id_declaration { get; set; }

        public string SenderId { get; set; }
        public string Sujet { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public string Status { get; set; } // "En attente", "Repondu"
    }
}
