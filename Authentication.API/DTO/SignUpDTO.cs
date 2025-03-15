using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.API.DTO
{
    public class SignUpDTO
    {
        public int CIN { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Adresse { get; set; }
        public string Role { get; set; }
        public string NumeroCompte { get; set; }
        public string MotDePasse { get; set; }
        public string ConfirmMotDePasse { get; set; }
         public string ImageUrl { get; set; }
        
    }

}
