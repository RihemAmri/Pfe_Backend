using Microsoft.AspNetCore.Http;

namespace Credit.API.DTOs
{
    public class UploadRecupayementDto
    {
        public IFormFile Fichier { get; set; }
    }
}
