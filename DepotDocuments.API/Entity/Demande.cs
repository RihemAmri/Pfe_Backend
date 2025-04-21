using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DepotDocuments.API.Entities
{
    public class Demande
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public decimal Montant { get; set; }
        public int Duree { get; set; }
        public string TypeCredit { get; set; }
        public string TypeFinancement { get; set; }
        public DateTime DateSoumission { get; set; }
    }
}
