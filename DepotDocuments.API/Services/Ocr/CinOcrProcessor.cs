using DepotDocuments.API.Services.Ocr.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Linq;

namespace DepotDocuments.API.Services.Ocr
{
    public class CinOcrProcessor : IOcrProcessor
    {
        private readonly OcrConfigService _ocrConfigService;
        private readonly HttpClient _httpClient;

        public CinOcrProcessor(OcrConfigService ocrConfigService, IHttpClientFactory httpClientFactory)
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
            resultRequest.Headers.Add("Ocp-Apim-Subscription-Key", _ocrConfigService.AzureKey);
            var resultResponse = await _httpClient.SendAsync(resultRequest);
            resultJson = await resultResponse.Content.ReadAsStringAsync();

            Console.WriteLine("Réponse Azure OCR : " + resultJson); // Debug

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

        // Extraire le numéro de CIN à partir du texte extrait
        var numeroCin = ExtraireNumeroCin(extractedText);

        return numeroCin ?? throw new Exception("Numéro CIN introuvable.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur OCR Azure : {ex.Message}");
        throw new Exception($"Erreur lors de l'extraction du texte avec Azure OCR : {ex.Message}", ex);
    }
}

private string ExtraireNumeroCin(string texte)
{
    // Expression régulière pour capturer un numéro CIN (8 chiffres consécutifs)
    var match = System.Text.RegularExpressions.Regex.Match(texte, @"\b\d{8}\b");
    return match.Success ? match.Value : null;
}
    }}