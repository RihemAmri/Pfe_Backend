using System.Text.Json.Serialization;

namespace Validation.API.Dtos
{
    public class ValidationRequestDto
    {
        public string NewStatus { get; set; }
        public string? SignatureAdminBase64 { get; set; }

        public string? MotifRefus { get; set; }
        [JsonPropertyName("UserId")]
        public string UserId { get; set; }
    }
}