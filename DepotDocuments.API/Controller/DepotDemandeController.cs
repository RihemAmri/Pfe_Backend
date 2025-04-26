using Microsoft.AspNetCore.Mvc;
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
    public class DepotDemandeController : ControllerBase
    {
        private readonly IMongoCollection<DepotDemande> _demandeCollection;
        private readonly MailService _mailService;
        private readonly IHttpClientFactory _httpClientFactory;
        
        //private readonly BrevoHttpMailService _mailService;
        //public DepotDemandeController(IConfiguration config, BrevoHttpMailService mailService)
        public DepotDemandeController(IConfiguration config, MailService mailService,IHttpClientFactory httpClientFactory)
        {
            var connectionString = config["DepotDemandeSettings:ConnectionString"];
            var databaseName = config["DepotDemandeSettings:DatabaseName"];
            var collectionName = config["DepotDemandeSettings:CollectionName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _demandeCollection = database.GetCollection<DepotDemande>(collectionName);
            _mailService = mailService;
            _httpClientFactory = httpClientFactory;
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
                DocumentIds = dto.DocumentIds ?? new List<string>()
            };

            await _demandeCollection.InsertOneAsync(depot);

            // Envoi de l’email de confirmation
            await _mailService.EnvoyerMailConfirmation(depot.Email, $"{depot.Prenom} {depot.Nom}");
            await _mailService.EnvoyerNotificationAdmin();
             // 🚀 Appel à Notification.API
        var client = _httpClientFactory.CreateClient("NotificationApi");
        var notif = new NotificationDto{
            DestinataireId = "6807f3958d2732dd1864cd9b", //⚠️nodnod badil lina 
            Message = $"Le client {depot.UserId} a demandé un crédit de {depot.TypeCredit} pour {depot.MontantDemande} TND",
            Date = DateTime.UtcNow,
            Lu = false,
            Type = "demande"
        };
        await client.PostAsJsonAsync("api/Notification", notif);

            //await _mailService.EnvoyerMailAsync(depot.Email, $"{depot.Prenom} {depot.Nom}");
            return Ok(new { message = "Demande enregistrée avec succès", id = depot.Id });
        }
    }
}
