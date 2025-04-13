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
    public class AttestationSalaireOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public AttestationSalaireOcrProcessor(OcrConfigService ocrConfigService, HttpClient httpClient)
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

                    if (line.Contains("total"))
                    {
                        Console.WriteLine($"Ligne détectée (total) : {lines[i]}");

                        // Cherche un montant sur la même ligne que "total"
                        var matchSameLine = Regex.Match(lines[i], @"\d[\d\s.,]{2,}");
                        if (matchSameLine.Success)
                        {
                            string montant = matchSameLine.Value.Replace(" ", "").Replace(",", ".");
                            return $"{montant}";
                        }

                        // Sinon, regarde la ligne suivante
                        if (i + 1 < lines.Length)
                        {
                            Console.WriteLine($"Ligne suivante : {lines[i + 1]}");
                            var matchNextLine = Regex.Match(lines[i + 1], @"\d[\d\s.,]{2,}");
                            if (matchNextLine.Success)
                            {
                                string montant = matchNextLine.Value.Replace(" ", "").Replace(",", ".");
                                return $"💰 Montant Total Brut Annuel : {montant}";
                            }
                        }

                        return "Ligne 'Total' trouvée, mais aucun montant clair n’a été détecté.";
                    }
                }

                Console.WriteLine("Texte OCR sans ligne contenant 'Total'. Voici le contenu brut :");
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }

                return "❌ Ligne 'Total' non trouvée dans l'attestation de salaire.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur OCR attestation de salaire : {ex.Message}");
                throw new Exception($"Erreur lors de l'extraction OCR attestation de salaire : {ex.Message}", ex);
            }
        }
    }
}
