/*using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DepotDocuments.API.DTO;
using DepotDocuments.API.Entities;
using DepotDocuments.API.Services;
using System.Net.Http;
using System.Net.Http.Json;
namespace DepotDocuments.API.Controllers
{

[ApiController]
[Route("api/[controller]")]
public class SignatureController : ControllerBase
{
    private readonly DocuSignService _docuSignService;

    public SignatureController(DocuSignService docuSignService)
    {
        _docuSignService = docuSignService;
    }

  [HttpPost("generer-lien")]
public async Task<IActionResult> GenererLienSignature([FromForm] SignatureUploadRequest request)
{
    var fichier = request.File;
    var email = request.Email;
    var nom = request.Nom;

    if (fichier == null || fichier.Length == 0)
        return BadRequest("Fichier manquant ou vide.");

    using var ms = new MemoryStream();
    await fichier.CopyToAsync(ms);
    var pdfBytes = ms.ToArray();

    var url = await _docuSignService.CreateEmbeddedSigning(email, nom, pdfBytes);
    return Ok(new { url });
}

}
}*/