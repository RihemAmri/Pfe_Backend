namespace Authentication.API.DTO;
public class UtilisateurProfilUpdateDto
{
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Adresse { get; set; }
    public IFormFile? ImageFile { get; set; } // Image facultative
}
