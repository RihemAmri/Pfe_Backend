using DepotDocuments.API.Services.Ocr.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;

namespace DepotDocuments.API.Services.Ocr
{
    public class AttestationSalaireOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public AttestationSalaireOcrProcessor(OcrConfigService ocrConfigService, IHttpClientFactory httpClientFactory)
        {
            _ocrConfigService = ocrConfigService;
            _httpClient = httpClientFactory.CreateClient("AzureOcr");
        }

        public async Task<string> ExtractTextAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                // Envoi de la demande à l'API Azure OCR
                var request = new HttpRequestMessage(HttpMethod.Post, "vision/v3.2/read/analyze")
                {
                    Content = new StreamContent(stream)
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // En-tête pour la clé d'API Azure
                var subscriptionKey = _ocrConfigService.AzureKey;
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Erreur Azure OCR (analyse initiale) : {response.StatusCode}");

                // Récupération de l'URL de la réponse de l'opération
                var operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                if (string.IsNullOrWhiteSpace(operationLocation))
                    throw new Exception("En-tête 'Operation-Location' manquant.");

                string resultJson = null;
                // Attente de la réponse finale après analyse
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000);
                    var resultRequest = new HttpRequestMessage(HttpMethod.Get, operationLocation);
                    var resultResponse = await _httpClient.SendAsync(resultRequest);
                    resultJson = await resultResponse.Content.ReadAsStringAsync();

                    Console.WriteLine("Réponse Azure OCR : " + resultJson); // Debug

                    // Vérification du statut de l'analyse
                    using var doc = JsonDocument.Parse(resultJson);
                    if (doc.RootElement.TryGetProperty("status", out JsonElement statusElement))
                    {
                        var status = statusElement.GetString();
                        if (status == "succeeded")
                            break;
                        if (status == "failed")
                            throw new Exception("Échec de l’analyse OCR.");
                    }
                }

                // Parsing de la réponse JSON finale
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

                // Appel à la méthode qui extrait le montant brut annuel
                var montantTotal = ExtraireMontantTotal(extractedText);
                return montantTotal ?? throw new Exception("Montant brut annuel introuvable.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur OCR attestation de salaire : {ex.Message}");
                throw new Exception($"Erreur lors de l'extraction OCR attestation de salaire : {ex.Message}", ex);
            }
        }

        // Méthode pour extraire le montant total brut annuel
        private string ExtraireMontantTotal(string texte)
{
    // Rechercher une ligne contenant "Total" suivi d'un montant
    var lignes = texte.Split('\n');

    foreach (var ligne in lignes)
    {
        if (ligne.Trim().ToLower().StartsWith("total"))
        {
            // Extraire le montant qui suit "Total"
            var match = Regex.Match(ligne, @"\d[\d\s.,]*");
            if (match.Success)
            {
                string montant = match.Value.Replace(" ", "").Replace(",", ".");
                return $"{montant}";
            }
        }
    }

    return null;
}

    }
}
