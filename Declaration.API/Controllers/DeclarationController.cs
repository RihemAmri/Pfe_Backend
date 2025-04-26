using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Declarations.API.Entities;
using Declarations.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Declarations.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeclarationController : ControllerBase
    {
        private readonly IMongoCollection<Declaration> _declarations;
        private readonly IMongoCollection<Reponse> _reponses;
        private readonly IHttpClientFactory _httpClientFactory;

        public DeclarationController(IMongoDatabase db, IHttpClientFactory httpClientFactory)
        {
            _declarations = db.GetCollection<Declaration>("declarations");
            _reponses = db.GetCollection<Reponse>("reponses");
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeclaration([FromBody] DeclarationDTO dto)
        {
            var declaration = new Declaration
            {
                SenderId = dto.SenderId,
                Sujet = dto.Sujet,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                Status = "En attente"
            };

            await _declarations.InsertOneAsync(declaration);

            // 🚀 Appel à Notification.API pour l'admin
            var client = _httpClientFactory.CreateClient("NotificationApi");
            var notif = new CreateNotificationDto
            {
                DestinataireId = "6807f3958d2732dd1864cd9b", // 🧠 ID Admin
                Message = $"Nouvelle déclaration reçue de l'utilisateur {dto.SenderId} : \"{dto.Sujet}\"",
                Date = DateTime.UtcNow,
                Lu = false,
                Type = "declaration"
            };
            await client.PostAsJsonAsync("api/Notification", notif);
              
            return Ok(declaration);
        }

        [HttpGet("mine/{userId}")]
        public async Task<IActionResult> GetUserDeclarations(string userId)
        {
            var declarations = await _declarations.Find(d => d.SenderId == userId).ToListAsync();
            var result = new List<object>();
            foreach (var dec in declarations)
            {
                var responses = await _reponses.Find(r => r.DeclarationId == dec.id_declaration).ToListAsync();
                result.Add(new { dec, responses });
            }
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllDeclarations()
        {
            var declarations = await _declarations.Find(_ => true).ToListAsync();
            return Ok(declarations);
        }

        [HttpPost("reponse")]
        public async Task<IActionResult> AddReponse([FromBody] ReponseDTO dto)
        {
            var reponse = new Reponse
            {
                DeclarationId = dto.DeclarationId,
                Message = dto.Message,
                SentAt = DateTime.UtcNow
            };

            await _reponses.InsertOneAsync(reponse);
            await _declarations.UpdateOneAsync(
                d => d.id_declaration == dto.DeclarationId,
                Builders<Declaration>.Update.Set(d => d.Status, "Repondu")
            );

            var declaration = await _declarations.Find(d => d.id_declaration == dto.DeclarationId).FirstOrDefaultAsync();
            if (declaration != null)
            {
                var client = _httpClientFactory.CreateClient("NotificationApi");
                var notif = new CreateNotificationDto
                {
                    DestinataireId = declaration.SenderId,
                    Message = $"Votre déclaration \"{declaration.Sujet}\" a reçu une réponse.",
                    Date = DateTime.UtcNow,
                    Lu = false,
                    Type = "repdeclaration"
                };
                await client.PostAsJsonAsync("api/Notification", notif);

                return Ok(reponse);
            }

            // Si la déclaration est introuvable
            return NotFound("Déclaration non trouvée");
        }
        [HttpGet("all-with-reponses")]
public async Task<IActionResult> GetAllDeclarationsWithReponses()
{
    var declarations = await _declarations.Find(_ => true).ToListAsync();
    var result = new List<object>();

    // Pour chaque déclaration, récupérer les réponses associées
    foreach (var dec in declarations)
    {
        var responses = await _reponses.Find(r => r.DeclarationId == dec.id_declaration).ToListAsync();
        result.Add(new { declaration = dec, responses });
    }

    return Ok(result);
}


        [HttpGet("thread/{id}")]
        public async Task<IActionResult> GetThread(string id)
        {
            var declaration = await _declarations.Find(d => d.id_declaration == id).FirstOrDefaultAsync();
            if (declaration == null)
                return NotFound();

            var responses = await _reponses.Find(r => r.DeclarationId == id).ToListAsync();
            return Ok(new { declaration, responses });
        }
    }
}
