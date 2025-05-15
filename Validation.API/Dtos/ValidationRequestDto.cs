namespace Validation.API.Dtos
{
    public class ValidationRequestDto
    {
        public string NewStatus { get; set; }
        public string SignatureAdminBase64 { get; set; } 
    }
}