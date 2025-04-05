using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Authentication.API.Entities
{
    public class Utilisateur
    {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public ObjectId Id { get; set; }

        [BsonElement("CIN")]
        public int CIN { get; set; }

            public string Nom { get; set; }

            public string Email { get; set; }

            public string Adresse { get; set; }

            public string MotDePasse { get; set; }
            public string NumeroCompte { get; set; }
            public string Role { get; set; }
            public string ImageUrl { get; set; }
             public string ResetToken { get; set; }
public DateTime? ResetTokenExpiration { get; set; }
        
    }


    }

