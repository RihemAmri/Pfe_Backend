using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DepotDocuments.API.Services
{
    public class YousignService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api-sandbox.yousign.app/v3/";
        private readonly string _apiKey = "mICVXGLI08pPURnoRje64IMbN8grLu0V"; // Remplace par ta vraie clé en prod

        public YousignService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> CreateSignatureRequestAsync(byte[] pdfBytes)
        {
            try
            {
                // Étape 1 : Upload du document PDF
                var fileContent = new ByteArrayContent(pdfBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                var multipartContent = new MultipartFormDataContent();
                multipartContent.Add(fileContent, "file", "demande.pdf");
                multipartContent.Add(new StringContent("signable_document"), "nature");

                var uploadResponse = await _httpClient.PostAsync("documents", multipartContent);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    var error = await uploadResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Yousign Upload Error: " + error);
                    throw new HttpRequestException($"Erreur upload: {uploadResponse.StatusCode} - {error}");
                }

                var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
                var fileId = JsonDocument.Parse(uploadJson).RootElement.GetProperty("id").GetString();

                // Étape 2 : Créer la demande de signature
                var signatureRequestBody = new
                {
                    name = "Demande de signature - Crédit",
                    documents = new[] { fileId },
                    delivery_mode = "email",
                    // Tu peux ajouter cette config plus tard si tu veux la redirection
                    // config = new {
                    //     redirect_success = "http://localhost:4200/demande/demande-credit?event=signing_complete"
                    // }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(signatureRequestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var signatureResponse = await _httpClient.PostAsync("signature_requests", content);

                if (!signatureResponse.IsSuccessStatusCode)
                {
                    var error = await signatureResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Yousign Signature Request Error: " + error);
                    throw new HttpRequestException($"Erreur signature request: {signatureResponse.StatusCode} - {error}");
                }

                var signatureJson = await signatureResponse.Content.ReadAsStringAsync();
                var root = JsonDocument.Parse(signatureJson).RootElement;

                if (!root.TryGetProperty("id", out var idProp))
                    throw new Exception("Réponse Yousign ne contient pas d'ID");

                var signatureRequestId = idProp.GetString();

                // Étape 3 : Récupérer l’URL de signature embarquée
                var embeddedResponse = await _httpClient.GetAsync($"signature_requests/{signatureRequestId}/embedded_url");

                if (!embeddedResponse.IsSuccessStatusCode)
                {
                    var error = await embeddedResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Yousign Embedded URL Error: " + error);
                    throw new HttpRequestException($"Erreur embedded URL: {embeddedResponse.StatusCode} - {error}");
                }

                var embeddedJson = await embeddedResponse.Content.ReadAsStringAsync();
                var embeddedUrl = JsonDocument.Parse(embeddedJson).RootElement.GetProperty("url").GetString();

                return embeddedUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur dans CreateSignatureRequestAsync : " + ex.Message);
                throw;
            }
        }
    }
}
