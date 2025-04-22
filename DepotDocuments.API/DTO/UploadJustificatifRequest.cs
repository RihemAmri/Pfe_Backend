using Microsoft.AspNetCore.Http;

namespace DepotDocuments.API.DTO
{
    public class UploadJustificatifRequest
    {
        public IFormFile File { get; set; }
    }
}
