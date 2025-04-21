namespace DepotDocuments.API.DTO
{
    public class CreditDocumentUploadDto
    {
        public string UserId { get; set; }

        public string FileType { get; set; }

        public string FileUrl { get; set; }

        public string ExtractedText { get; set; }
    }
}
