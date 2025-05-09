using DepotDocuments.API.Services.Ocr.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DepotDocuments.API.Services.Ocr
{
    public class CinOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public CinOcrProcessor(OcrConfigService ocrConfigService, HttpClient httpClient)
        {
            _ocrConfigService = ocrConfigService;
            _httpClient = httpClient;
        }

        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();

                content.Add(new StreamContent(stream), "file", file.FileName);
                content.Add(new StringContent(_ocrConfigService.OcrApiKey), "apikey");
                content.Add(new StringContent("false"), "isOverlayRequired");

                var response = await _httpClient.PostAsync(_ocrConfigService.OcrApiUrl, content);
                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"OCR Response: {json}");

                dynamic result = JsonConvert.DeserializeObject(json);

                if (result?.IsErroredOnProcessing == true)
                {
                    var errorMessage = result?.ErrorMessage?.ToString() ?? "Erreur inconnue OCR";
                    throw new Exception("OCR.space error: " + errorMessage);
                }

                var extractedText = result?.ParsedResults?[0]?.ParsedText?.ToString();

                if (string.IsNullOrWhiteSpace(extractedText))
                {
                    throw new Exception("Aucun texte extrait de l'image.");
                }

                // ✅ Prend uniquement la première ligne du texte
                var lines = extractedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var firstLine = lines.Length > 0 ? lines[0] : string.Empty;

                return firstLine;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during OCR extraction: {ex.Message}");
                throw new Exception($"Erreur lors de l'extraction du texte OCR : {ex.Message}", ex);
            }
        }
    }
}