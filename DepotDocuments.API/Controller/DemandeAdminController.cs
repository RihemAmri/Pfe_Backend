using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using DepotDocuments.API.Entities;
using DepotDocuments.API.DTO;


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
[HttpPut("update-statut/{id}")]
public async Task<IActionResult> UpdateStatutDemande(string id, [FromBody] string nouveauStatut, [FromServices] RabbitMQProducer producer)
{
    var update = Builders<DepotDemande>.Update.Set(d => d.Statut, nouveauStatut);
    var result = await _demandeCollection.UpdateOneAsync(d => d.Id == id, update);

    if (result.MatchedCount == 0)
        return NotFound($"Aucune demande trouvée avec l'ID : {id}");

    // Récupérer la demande mise à jour
    var demande = await _demandeCollection.Find(d => d.Id == id).FirstOrDefaultAsync();

    // Si statut devient "crédit actif", envoyer via RabbitMQ
    if (nouveauStatut == "crédit actif" && demande != null)
    {
        var message = new CreditMessageDto
        {
            IdDemande = demande.Id,
            IdClient = demande.UserId,
            Montant = demande.MontantDemande,
            DureeMois = demande.DureeEnAnnees * 12,
            TypeCredit = demande.TypeCredit
        };

        producer.SendMessage(message);
    }

    return Ok($"Statut de la demande {id} mis à jour avec succès.");
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
