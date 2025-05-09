using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DepotDocuments.API.DTO;
using DepotDocuments.API.Entities;
using DepotDocuments.API.Services;
using DepotDocuments.API.Services.Ocr.Interfaces;

using System.Net.Http;
using System.Net.Http.Json;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.Text.RegularExpressions;
namespace DepotDocuments.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepotDemandeController : ControllerBase
    {
        private readonly IMongoCollection<DepotDemande> _demandeCollection;
        private readonly MailService _mailService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConverter _pdfConverter;
        private readonly DocuSignService _docusignService;
        private readonly PdfService _pdfService;

        private readonly YousignService  _yousignService;
        //private readonly BrevoHttpMailService _mailService;
        //public DepotDemandeController(IConfiguration config, BrevoHttpMailService mailService)
        public DepotDemandeController(IConfiguration config, MailService mailService,IHttpClientFactory httpClientFactory,IConverter pdfConverter,PdfService pdfService,DocuSignService docusignService,YousignService  yousignService)
{
        
            var connectionString = config["DepotDemandeSettings:ConnectionString"];
            var databaseName = config["DepotDemandeSettings:DatabaseName"];
            var collectionName = config["DepotDemandeSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _demandeCollection = database.GetCollection<DepotDemande>(collectionName);
            _mailService = mailService;
            _httpClientFactory = httpClientFactory;
            _pdfConverter = pdfConverter;
             _pdfService = pdfService;
             _docusignService = docusignService;
             _yousignService = yousignService;
        }
        

        [HttpPost]
        public async Task<IActionResult> EnregistrerDepotDemande([FromBody] DepotDemandeDto dto)
        {
            var depot = new DepotDemande
            {
                UserId = dto.UserId,
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                Email = dto.Email,
                Adresse = dto.Adresse,
                Cin = dto.Cin,
                NumeroCompte = dto.NumeroCompte,
                DateNaissance = dto.DateNaissance,
                Telephone = dto.Telephone,
                Civilite = dto.Civilite,
                MontantDemande = dto.MontantDemande,
                MensualiteEstimee = dto.MensualiteEstimee,
                DureeEnAnnees = dto.DureeEnAnnees,
                TypeCredit = dto.TypeCredit,
                TypeFinancement = dto.TypeFinancement,
                RevenuMensuelOcr = dto.RevenuMensuelOcr,
                AttestationSalaireOcr = dto.AttestationSalaireOcr,
                DocumentIds = dto.DocumentIds ?? new List<string>(),
                DateDerniereModification = DateTime.UtcNow
            };

            await _demandeCollection.InsertOneAsync(depot);

            // Envoi de l’email de confirmation
            //await _mailService.EnvoyerMailConfirmation(depot.Email, $"{depot.Prenom} {depot.Nom}");
            if (!string.IsNullOrEmpty(dto.PdfBase64))
            {
                byte[] pdfBytes = Convert.FromBase64String(dto.PdfBase64);
                await _mailService.EnvoyerMailConfirmationAvecPdf(depot.Email, $"{depot.Prenom} {depot.Nom}", pdfBytes);
            }
            else
            {
                await _mailService.EnvoyerMailConfirmation(depot.Email, $"{depot.Prenom} {depot.Nom}");
            }
            await _mailService.EnvoyerNotificationAdmin();
             // 🚀 Appel à Notification.API
        var client = _httpClientFactory.CreateClient("NotificationApi");
        var notif = new NotificationDto{
            DestinataireId = "6807f3958d2732dd1864cd9b", //⚠️nodnod badil lina 
            Message = $"Le client {depot.NumeroCompte} a demandé un crédit de {depot.TypeCredit} pour {depot.MontantDemande} TND",
            Date = DateTime.UtcNow,
            Lu = false,
            Type = "demande"
        };
        await client.PostAsJsonAsync("api/Notification", notif);

            //await _mailService.EnvoyerMailAsync(depot.Email, $"{depot.Prenom} {depot.Nom}");
            return Ok(new { message = "Demande enregistrée avec succès", id = depot.Id });
        }
     [HttpPost("generer-pdf")]
    public IActionResult GenererPdf([FromBody] DepotDemandeDto demande)
    {
        var htmlContent = $@"
            <html>
            <head><style>body {{ font-family: Arial; }}</style></head>
            <body>
                <h1>Demande de crédit</h1>
                <p><strong>Nom :</strong> {demande.Nom}</p>
                <p><strong>Prénom :</strong> {demande.Prenom}</p>
                <p><strong>CIN :</strong> {demande.Cin}</p>
                <p><strong>Montant demandé :</strong> {demande.MontantDemande} DT</p>
                <p><strong>Durée :</strong> {demande.DureeEnAnnees} années</p>
                <p><strong>Email :</strong> {demande.Email}</p>
                <p><strong>Type de crédit :</strong> {demande.TypeCredit}</p>
            </body>
            </html>";

        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
            Objects = {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };

        var pdfBytes = _pdfConverter.Convert(doc);

        var fileName = $"demande-{Guid.NewGuid()}.pdf";
        var path = Path.Combine("Documents", fileName);

        if (!Directory.Exists("Documents"))
            Directory.CreateDirectory("Documents");

        System.IO.File.WriteAllBytes(path, pdfBytes);

        return Ok(new { fileName, path });
    }
    [HttpPost("signature")]
    public IActionResult SignDocument([FromBody] SignatureUploadRequest request)
    {   ModelState.Clear();

            // ✅ Validation conditionnelle
        if (request.Type == "drawn" && string.IsNullOrEmpty(request.DrawnSignature))
        {
            return BadRequest(new
            {
                errors = new { DrawnSignature = new[] { "The DrawnSignature field is required." } }
            });
        }

        if (request.Type == "uploaded" && string.IsNullOrEmpty(request.UploadedSignature))
        {
            return BadRequest(new
            {
                errors = new { UploadedSignature = new[] { "The UploadedSignature field is required." } }
            });
        }

        if (request.Demande == null)
        {
            return BadRequest(new
            {
                errors = new { Demande = new[] { "La demande est requise." } }
            });
        }
        /*if (string.IsNullOrEmpty(base64) || request.Demande == null)
            return BadRequest("Signature ou données de demande manquantes");
        */
        string base64 = request.Type == "drawn" ? request.DrawnSignature : request.UploadedSignature;
        string base64Data = Regex.Replace(base64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
        byte[] signatureBytes = Convert.FromBase64String(base64Data);

        // Générer le PDF avec infos + signature
        byte[] pdf = _pdfService.GenerateDemandePdfWithSignature(request.Demande, signatureBytes);
        return File(pdf, "application/pdf");
        //return File(pdf, "application/pdf", "demande-signee.pdf");
    }



    [HttpPost("demande/generate-and-sign")]
    public async Task<IActionResult> GeneratePdfAndSign([FromBody] DepotDemandeDto demande)
    {
        // 1. Générer le PDF avec les infos dans `demande`
        byte[] pdfBytes = _pdfService.GenerateDemandePdf(demande);

        // 2. Créer une enveloppe Yousign avec ce PDF et configurer l’URL de signature intégrée
        var embeddedUrl = await _yousignService.CreateSignatureRequestAsync(pdfBytes);
        return Ok(new { signingUrl = embeddedUrl });
    }


    }
   
}
