using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Authentication.API.Entities
{
    public class Compte
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdCompte { get; set; }

        public string NumeroCompte { get; set; }
       
        public DateTime DateActivation { get; set; }
        public string Status { get; set; }
        public double Salaire { get; set; }

        public string HistoriqueCredit { get; set; } 
    }
}
