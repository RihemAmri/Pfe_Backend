using DepotDocuments.API.Services.Ocr.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DepotDocuments.API.Services.Ocr
{
    public class FichePaieOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public FichePaieOcrProcessor(OcrConfigService ocrConfigService, HttpClient httpClient)
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

                string[] lines = extractedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].ToLower();

                    if (line.Contains("net a payer") || line.Contains("net à payer"))
                    {
                        Console.WriteLine($"Ligne détectée : {lines[i]}");

                        // 1. Essayer de récupérer directement un montant sur la même ligne
                        var matchSameLine = Regex.Match(lines[i], @"\d[\d\s.,]{2,}");
                        if (matchSameLine.Success)
                        {
                            string montant = matchSameLine.Value.Replace(" ", "").Replace(",", ".");
                            return $"{montant}";
                        }

                        // 2. Sinon, essayer la ligne suivante s’il y en a une
                        if (i + 1 < lines.Length)
                        {
                            Console.WriteLine($"Ligne suivante : {lines[i + 1]}");
                            var matchNextLine = Regex.Match(lines[i + 1], @"\d[\d\s.,]{2,}");
                            if (matchNextLine.Success)
                            {
                                string montant = matchNextLine.Value.Replace(" ", "").Replace(",", ".");
                                return $"{montant}";
                            }
                        }

                        return "Ligne détectée mais aucun montant clair trouvé.";
                    }
                }

                // Aucun "Net à Payer" trouvé
                Console.WriteLine("Texte OCR non interprété correctement. Voici le contenu brut :");
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }

                return "❌ Ligne 'Net à Payer' non trouvée dans le document OCR.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur OCR fiche de paie : {ex.Message}");
                throw new Exception($"Erreur lors de l'extraction OCR fiche de paie : {ex.Message}", ex);
            }
        }
    }
}
