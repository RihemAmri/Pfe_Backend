using Microsoft.AspNetCore.Mvc;
using Credit.API.Services;
using Credit.API.DTOs;
namespace Credit.API.Controllers


{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly ICreditService _service;
        private readonly IHttpClientFactory _httpClientFactory;

        public CreditController(ICreditService service,IHttpClientFactory httpClientFactory)
        {
            _service = service;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> AjouterCredit([FromBody] CreateCreditDto credit)
        {
            var result = await _service.AjouterCreditAsync(credit);
            return Ok(result);
        }

        [HttpGet("client/{idClient}")]
        public async Task<IActionResult> GetCreditsClient(string idClient)
        {
            var result = await _service.GetCreditsClientAsync(idClient);
            return Ok(result);
        }
        [HttpGet("all-sans-amortissement")]
        public async Task<IActionResult> GetAllCreditsSansAmortissement()
        {
            var result = await _service.GetAllCreditsSansAmortissementAsync();
            return Ok(result);
        }
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetCreditsParStatus(string status)
        {
            var result = await _service.GetCreditsParStatusAsync(status);
            return Ok(result);
        }
        [HttpGet("type/{typeCredit}")]
        [HttpGet("type-sans-amortissement/{typeCredit}")]
public async Task<IActionResult> GetCreditsParTypeSansAmortissement(string typeCredit)
{
    var result = await _service.GetCreditsParTypeSansAmortissementAsync(typeCredit);
    return Ok(result);
}


[HttpPut("cloturer/{id}")]
public async Task<IActionResult> CloturerCredit(string id)
{
    var result = await _service.CloturerCreditAsync(id);
    if (!result)
    {
        return BadRequest("Impossible de clôturer ce crédit.");
    }
    var credit = await _service.GetCreditParIdAsync(id);
    if (credit != null)
    {
        var client = _httpClientFactory.CreateClient("NotificationApi");
        var notif = new CreateNotificationDto
        {
            DestinataireId = credit.IdClient, // Assure-toi que `IdClient` est bien accessible
            Message = $"Votre crédit \"{credit.TypeCredit}\" est clôturé. Merci pour votre fidélité.",
            Date = DateTime.UtcNow,
            Lu = false,
            Type = "credit"
        };

       await client.PostAsJsonAsync("api/Notification", notif);
    }

    return Ok(new { message = "Crédit clôturé avec succès." });
}




        [HttpPost("mise-a-jour/{idCredit}")]
        public async Task<IActionResult> MettreAJourParId(string idCredit)
        {
            await _service.MettreAJourAmortissementParIdAsync(idCredit);
            return Ok($"Mise à jour du crédit {idCredit} effectuée");
        }
        [HttpPost("payer-integralement/{idCredit}")]
public async Task<IActionResult> PayerIntegralementCredit(string idCredit)
{
    var result = await _service.PayerIntegralementCreditAsync(idCredit);
    if (!result)
        return NotFound($"Crédit avec ID {idCredit} non trouvé.");

    return Ok($"Crédit {idCredit} payé intégralement.");
}
[HttpGet("{idCredit}")]
public async Task<IActionResult> GetCreditParId(string idCredit)
{
    var result = await _service.GetCreditParIdAsync(idCredit);
    if (result == null)
        return NotFound($"Crédit avec ID {idCredit} introuvable.");
    
    return Ok(result);
}
[HttpPost("mise-a-jour-automatique")]
public async Task<IActionResult> MettreAJourAmortissements()
{
    var anciensCredits = await _service.GetCreditsParStatusAsync("EnCours"); // ou "en cours", selon ton nom

    // 2. Effectuer la mise à jour
    await _service.MettreAJourAmortissementsAsync();

    // 3. Récupérer à nouveau tous les crédits
    var nouveauxCredits = await _service.GetCreditsParStatusAsync("Cloture");

    // 4. Trouver les crédits qui étaient dans les anciens mais qui sont maintenant clôturés
    var cloturesRecents = nouveauxCredits
        .Where(nouveau => anciensCredits.Any(ancien => ancien.Id == nouveau.Id))
        .ToList();

    // 5. Envoyer une notification pour chaque crédit devenu clôturé
    var client = _httpClientFactory.CreateClient("NotificationApi");

    foreach (var credit in cloturesRecents)
    {
        var notif = new CreateNotificationDto
        {
            DestinataireId = credit.IdClient,
            Message = $"Votre crédit \"{credit.TypeCredit}\" est désormais clôturé. Merci pour votre fidélité.",
            Date = DateTime.UtcNow,
            Lu = false,
            Type = "credit"
        };

        await client.PostAsJsonAsync("api/Notification", notif);
    }
    
    return Ok("Mise à jour effectuée avec succès.");
}


    }
}
