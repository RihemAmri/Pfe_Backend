namespace DepotDocuments.API.Services.Ocr
{
    public class OcrConfigService
    {
        public string OcrApiKey { get; private set; }
        public string OcrApiUrl { get; private set; }

        public OcrConfigService()
        {
            // Initialisation avec ta clé API OCR.space
            OcrApiKey = "K81831125588957"; // Ta clé API OCR.space
            OcrApiUrl = "https://api.ocr.space/parse/image"; // URL de l'API OCR.space
        }
    }
}
