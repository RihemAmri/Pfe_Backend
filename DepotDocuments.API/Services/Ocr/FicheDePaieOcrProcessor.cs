using DepotDocuments.API.Services.Ocr.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace DepotDocuments.API.Services.Ocr
{
    public class FichePaieOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public FichePaieOcrProcessor(OcrConfigService ocrConfigService, IHttpClientFactory httpClientFactory)
        {
            _ocrConfigService = ocrConfigService;
            _httpClient = httpClientFactory.CreateClient("AzureOcr");
        }

        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                var request = new HttpRequestMessage(HttpMethod.Post, "vision/v3.2/read/analyze")
                {
                    Content = new StreamContent(stream)
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _ocrConfigService.AzureKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Erreur Azure OCR (analyse initiale) : {response.StatusCode}");

                var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                if (string.IsNullOrWhiteSpace(operationLocation))
                    throw new Exception("En-tête 'Operation-Location' manquant.");

                string resultJson = null;
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000);
                    var resultRequest = new HttpRequestMessage(HttpMethod.Get, operationLocation);
                    var resultResponse = await _httpClient.SendAsync(resultRequest);
                    resultJson = await resultResponse.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(resultJson);
                    if (doc.RootElement.TryGetProperty("status", out var statusElement))
                    {
                        var status = statusElement.GetString();
                        if (status == "succeeded")
                            break;
                        if (status == "failed")
                            throw new Exception("Échec de l’analyse OCR.");
                    }
                }

                using var finalDoc = JsonDocument.Parse(resultJson);
                if (!finalDoc.RootElement.TryGetProperty("analyzeResult", out var analyzeResult))
                    throw new Exception("Clé 'analyzeResult' introuvable dans la réponse Azure OCR.");

                if (!analyzeResult.TryGetProperty("readResults", out var readResults) || readResults.GetArrayLength() == 0)
                    throw new Exception("Clé 'readResults' vide ou introuvable.");

                var lines = readResults[0].GetProperty("lines");
                string extractedText = "";
                foreach (var line in lines.EnumerateArray())
                {
                    if (line.TryGetProperty("text", out var textElement))
                    {
                        extractedText += textElement.GetString() + "\n";
                    }
                }

                Console.WriteLine("Texte OCR brut :\n" + extractedText); // Pour debug

                var salaireBrut = ExtraireSalaireBrut(extractedText);
                return salaireBrut ?? throw new Exception("Montant 'Salaire Brut' non trouvé.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur OCR fiche de paie : {ex.Message}");
                throw new Exception($"Erreur lors de l'extraction OCR fiche de paie : {ex.Message}", ex);
            }
        }

        private string ExtraireSalaireBrut(string texte)
        {
            var lignes = texte.Split('\n');
            for (int i = 0; i < lignes.Length; i++)
            {
                var ligne = lignes[i].Trim().ToLower();

                // Cas 1 : "Salaire Brut" simple
                if (ligne == "salaire brut")
                {
                    int montantTrouves = 0;
                    for (int j = 1; j <= 3 && i + j < lignes.Length; j++)
                    {
                        var montantMatchs = Regex.Matches(lignes[i + j], @"\d[\d.,]{2,}");
                        foreach (Match match in montantMatchs)
                        {
                            montantTrouves++;
                            if (montantTrouves == 2) // prendre le 2e montant
                            {
                                var montant = match.Value.Replace(",", ".").Replace(" ", "");
                                return $"{montant}";
                            }
                        }
                    }
                }

                // Cas 2 : ligne avec code (ex: "18 - SALAIRE BRUT")
                if (Regex.IsMatch(ligne, @"\b\d+\s*-\s*salaire brut\b"))
                {
                    if (i + 1 < lignes.Length)
                    {
                        var montantMatch = Regex.Match(lignes[i + 1], @"\d[\d.,]{2,}");
                        if (montantMatch.Success)
                        {
                            var montant = montantMatch.Value.Replace(",", ".").Replace(" ", "");
                            return $"{montant}";
                        }
                    }
                }
            }

            return "❌ Salaire brut non trouvé.";
        }
    }
}
