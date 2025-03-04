using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Authentication.API.Entities
{
    public class Utilisateur
    {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string Id { get; set; }

            public int CIN { get; set; }

            public string Nom { get; set; }

            public string Email { get; set; }

            public string Adresse { get; set; }

            public string MotDePasse { get; set; }

            public string Role { get; set; }
        }


    }

