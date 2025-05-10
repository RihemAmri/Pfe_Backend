namespace DepotDocuments.API.Services.Ocr
{
    public class OcrConfigService
    {
        public string AzureKey { get; private set; }
        public string AzureEndpoint { get; private set; }

        public OcrConfigService()
        {
            AzureKey = Environment.GetEnvironmentVariable("AZURE_OCR_KEY") ?? throw new Exception("AZURE_OCR_KEY non défini.");
            AzureEndpoint = Environment.GetEnvironmentVariable("AZURE_OCR_ENDPOINT") ?? throw new Exception("AZURE_OCR_ENDPOINT non défini.");

            // Facultatif : pour le debug en local
            Console.WriteLine($"AzureKey: {(string.IsNullOrEmpty(AzureKey) ? "MISSING" : "LOADED")}");
            Console.WriteLine($"AzureEndpoint: {AzureEndpoint}");
        }
    }
}
