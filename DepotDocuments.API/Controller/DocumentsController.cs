using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DepotDocuments.API.DTO;
using DepotDocuments.API.Entities;
using DepotDocuments.API.Services.Ocr;
using DepotDocuments.API.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using DepotDocuments.API.Router;

namespace DepotDocuments.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly CloudinaryService _uploadService;
        private readonly OcrDispatcherService _ocrDispatcher;
        private readonly IDocumentContext _documentContext;

        public DocumentsController(
            CloudinaryService uploadService,
            OcrDispatcherService ocrDispatcher,
            IDocumentContext documentContext)  // Injecter le DocumentContext
        {
            _uploadService = uploadService;
            _ocrDispatcher = ocrDispatcher;
            _documentContext = documentContext; // Injecter la dépendance MongoDB via DocumentContext
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Fichier manquant.");

            // 1. Upload vers Cloudinary
            var uploadResult = await _uploadService.UploadImageAsync(request.File);
            var imageUrl = uploadResult.SecureUrl.ToString();

            // 2. Traitement OCR
            var extractedText = await _ocrDispatcher.ProcessAsync(request.File, request.TypeDocument);

            // 3. Sauvegarde en base MongoDB
            var document = new Document
            {
                Url = imageUrl,
                Type = request.TypeDocument,
                TextExtrait = extractedText,
                DateAjout = DateTime.UtcNow
            };

            var collection = _documentContext.Documents;
            await collection.InsertOneAsync(document);  // Insérer le document dans la collection MongoDB

            // 4. Retour au frontend
            return Ok(new OcrResultDTO
            {
                ImageUrl = imageUrl,
                ExtractedText = extractedText
            });
        }
    }
}
