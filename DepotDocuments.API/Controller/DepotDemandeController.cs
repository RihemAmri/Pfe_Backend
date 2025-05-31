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
        private readonly CloudinaryService _uploadService;
        //private readonly SignatureService _signatureService;
        public DepotDemandeController(IConfiguration config, MailService mailService, IHttpClientFactory httpClientFactory, IConverter pdfConverter, PdfService pdfService, DocuSignService docusignService, YousignService yousignService, CloudinaryService uploadService)
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
            _uploadService = uploadService;
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

    //await _demandeCollection.InsertOneAsync(depot);

    // ✅ Enregistrer la signature maintenant
    if (!string.IsNullOrEmpty(dto.Type))
    {
        string base64 = dto.Type == "drawn" ? dto.DrawnSignature : dto.UploadedSignature;

        if (!string.IsNullOrEmpty(base64))
        {
            string base64Data = Regex.Replace(base64, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
            byte[] signatureBytes = Convert.FromBase64String(base64Data);

            using var stream = new MemoryStream(signatureBytes);
            IFormFile fakeFile = new FormFile(stream, 0, signatureBytes.Length, "signature", "signature.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            string uploadedUrl = await _uploadService.UploadFileAsync(fakeFile);

            depot.SignatureUrl = uploadedUrl;
            depot.Type = dto.Type;

            //await _signatureService.SaveSignatureAsync(record);
        }
    }
    await _demandeCollection.InsertOneAsync(depot);
    // 📧 Email
    if (!string.IsNullOrEmpty(dto.PdfBase64))
    {
        byte[] pdfBytes = Convert.FromBase64String(dto.PdfBase64);
        await _mailService.EnvoyerMailConfirmationAvecPdf(depot.Email, $"{depot.Prenom} {depot.Nom}", pdfBytes);
    }
    else
    {
        await _mailService.EnvoyerMailConfirmation(depot.Email, $"{depot.Prenom} {depot.Nom}");
    }

    // 🔔 Notification admin
    var client = _httpClientFactory.CreateClient("NotificationApi");
    var notif = new NotificationDto
    {
        DestinataireId = "6807f3958d2732dd1864cd9b", // à remplacer dynamiquement si besoin
        Message = $"Le client {depot.NumeroCompte} a demandé un crédit de {depot.TypeCredit} pour {depot.MontantDemande} TND",
        Date = DateTime.UtcNow,
        Lu = false,
        Type = "demande"
    };
    await client.PostAsJsonAsync("api/Notification", notif);

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
    public async Task<IActionResult> SignDocument([FromBody] SignatureUploadRequest request)
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

        // Convertir le tableau d'octets en stream pour upload
    using var stream = new MemoryStream(signatureBytes);
    IFormFile fakeFile = new FormFile(stream, 0, signatureBytes.Length, "signature", "signature.png")
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/png"
    };

    // 📤 Upload sur Cloudinary
    /*string uploadedUrl = await _uploadService.UploadFileAsync(fakeFile);

    // 🗃️ Enregistrer la signature dans MongoDB
    var record = new SignatureRecord
    {
        DemandeId = request.Demande.Id,
        SignatureType = request.Type,
        CloudinaryUrl = uploadedUrl,
        Date = DateTime.UtcNow
    };

    await _signatureService.SaveSignatureAsync(record);*/


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