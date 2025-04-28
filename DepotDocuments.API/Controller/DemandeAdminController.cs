using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DepotDocuments.API.Entities;

namespace DepotDocuments.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DemandeAdminController : ControllerBase
    {
        private readonly IMongoCollection<DepotDemande> _demandeCollection;

        public DemandeAdminController(IConfiguration config)
        {
            var connectionString = config["DepotDemandeSettings:ConnectionString"];
            var databaseName = config["DepotDemandeSettings:DatabaseName"];
            var collectionName = config["DepotDemandeSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _demandeCollection = database.GetCollection<DepotDemande>(collectionName);
        }

        [HttpGet("all")]
public async Task<IActionResult> GetAllDemandes()
{
    try
    {
        var demandes = await _demandeCollection.Find(_ => true).ToListAsync();
        return Ok(demandes);
    }
    catch (Exception ex)
    {
        // Log l'erreur ici (ou retourne un message d'erreur)
        return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
    }
}
    [HttpGet("statut/{statut}")]
    public async Task<IActionResult> GetDemandesByStatut(string statut)
    {
    var demandes = await _demandeCollection.Find(d => d.Statut == statut).ToListAsync();
    return Ok(demandes);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDemandeById(string id)
    {
        var demande = await _demandeCollection.Find(d => d.Id == id).FirstOrDefaultAsync();
        return demande != null ? Ok(demande) : NotFound();
    }
        }
        
}
