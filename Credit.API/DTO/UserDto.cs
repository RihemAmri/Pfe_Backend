namespace Credit.API.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public int CIN { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string NumeroCompte { get; set; }
    }
}