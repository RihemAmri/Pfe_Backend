using Microsoft.AspNetCore.Mvc;
using DepotDocuments.API.Data;
using DepotDocuments.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DepotDocuments.API.DTO;
using MongoDB.Bson;
namespace DepotDocuments.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DepotFinalController : ControllerBase
    {
        private readonly ICreditDocumentService _creditDocumentService;

        public DepotFinalController(ICreditDocumentService creditDocumentService)
        {
             _creditDocumentService = creditDocumentService;
        }

    [HttpPost("uploadMultiple")]
    public async Task<IActionResult> UploadMultipleDocuments([FromBody] List<CreditDocumentUploadDto> docs)
    {
        try
        {
            var documents = docs.Select(dto => new CreditDocument
            {
                UserId = dto.UserId,
                FileType = dto.FileType,
                FileUrl = dto.FileUrl,
                ExtractedText = dto.ExtractedText
            }).ToList();

            await _creditDocumentService.AddDocumentsAsync(documents);

           // Préparer les données à retourner
            var response = documents.Select(doc => new
            {
                Id = doc.Id,
                FileType = doc.FileType,
                FileUrl = doc.FileUrl
            });

        return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
   [HttpPost("by-ids")]
    public async Task<IActionResult> GetDocumentsByIds([FromBody] List<string> ids)
    {
        try
        {
            var documents = await _creditDocumentService.GetDocumentsByIdsAsync(ids);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}}
