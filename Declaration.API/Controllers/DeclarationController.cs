// Fichier: Notification.API/Controllers/DeclarationController.cs
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Notification.API.Entities;
using Notification.API.DTO;
using System;
using System.Threading.Tasks;

namespace Notification.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeclarationController : ControllerBase
    {
        private readonly IMongoCollection<Declaration> _declarations;

        public DeclarationController(IMongoDatabase database)
        {
            _declarations = database.GetCollection<Declaration>("declarations");
        }

        // POST: api/declaration
        [HttpPost]
        public async Task<IActionResult> CreateDeclaration([FromBody] DeclarationDTO dto)
        {
            var declaration = new Declaration
            {
                SenderId = dto.SenderId,
                RecipientId = dto.RecipientId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow,
                IsReadByRecipient = dto.IsReadByRecipient,
                Status = dto.Status
            };

            await _declarations.InsertOneAsync(declaration);
            return CreatedAtAction(nameof(GetDeclaration), new { id = declaration.Id_Declaration }, declaration);
        }

        // GET: api/declaration/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeclaration(string id)
        {
            var declaration = await _declarations
                .Find(d => d.Id_Declaration == id)
                .FirstOrDefaultAsync();

            if (declaration == null)
                return NotFound();

            return Ok(declaration);
        }

        // PUT: api/declaration/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var filter = Builders<Declaration>.Filter.Eq(d => d.Id_Declaration, id);
            var update = Builders<Declaration>.Update.Set(d => d.IsReadByRecipient, true);

            var result = await _declarations.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0)
                return NotFound();

            return NoContent();
        }

        // GET: api/declaration
        [HttpGet]
        public async Task<IActionResult> GetAllDeclarations()
        {
            var declarations = await _declarations.Find(_ => true).ToListAsync();
            return Ok(declarations);
        }

        // DELETE: api/declaration/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeclaration(string id)
        {
            var filter = Builders<Declaration>.Filter.Eq(d => d.Id_Declaration, id);
            var result = await _declarations.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                return NotFound();

            return NoContent();
        }

        // GET: api/declaration/sender/{senderId}
        [HttpGet("sender/{senderId}")]
        public async Task<IActionResult> GetDeclarationsBySender(string senderId)
        {
            var declarations = await _declarations
                .Find(d => d.SenderId == senderId)
                .ToListAsync();

            if (declarations == null || declarations.Count == 0)
                return NotFound();

            return Ok(declarations);
        }

        // GET: api/declaration/recipient/{recipientId}
        [HttpGet("recipient/{recipientId}")]
        public async Task<IActionResult> GetDeclarationsByRecipient(string recipientId)
        {
            var declarations = await _declarations
                .Find(d => d.RecipientId == recipientId)
                .ToListAsync();

            if (declarations == null || declarations.Count == 0)
                return NotFound();

            return Ok(declarations);
        }

        // GET: api/declaration/sender/{senderId}/recipient/{recipientId}
        [HttpGet("sender/{senderId}/recipient/{recipientId}")]
        public async Task<IActionResult> GetDeclarationsBySenderAndRecipient(string senderId, string recipientId)
        {
            var declarations = await _declarations
                .Find(d => d.SenderId == senderId && d.RecipientId == recipientId)
                .ToListAsync();

            if (declarations == null || declarations.Count == 0)
                return NotFound();

            return Ok(declarations);
        }
    }
}
