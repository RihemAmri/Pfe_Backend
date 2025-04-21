using System.Collections.Generic;

namespace DepotDocuments.API.DTO
{
    public class FinalSubmissionRequest
    {
        public List<UploadedDocumentDTO> Documents { get; set; }
        public DemandeDTO Demande { get; set; }
    }

    public class UploadedDocumentDTO
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string TextExtrait { get; set; }
    }

    public class DemandeDTO
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public decimal Montant { get; set; }
        public int Duree { get; set; }
        public string TypeCredit { get; set; }
        public string TypeFinancement { get; set; }
    }
}

