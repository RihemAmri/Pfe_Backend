using Microsoft.AspNetCore.Http;

namespace DepotDocuments.API.DTO
{
   public class SignatureUploadRequest
{   
    public DepotDemandeDto Demande { get; set; }
    public string Type { get; set; } // "drawn" ou "uploaded"
    public string? DrawnSignature { get; set; } // Base64
    public string? UploadedSignature { get; set; } // Base64
}
}
