using Authentication.API.Entities;
using Authentication.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Authentication.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUtilisateurRepository _repository;

        public AuthController(IUtilisateurRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Utilisateur>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            var utilisateurs = await _repository.GetUtilisateurs();
            return Ok(utilisateurs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Utilisateur), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Utilisateur>> GetUtilisateurById(string id)
        {
            var utilisateur = await _repository.GetUtilisateurById(id);
            if (utilisateur == null)
                return NotFound();

            return Ok(utilisateur);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Utilisateur), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateUtilisateur([FromBody] Utilisateur utilisateur)
        {
            await _repository.CreateUtilisateur(utilisateur);
            return CreatedAtAction(nameof(GetUtilisateurById), new { id = utilisateur.Id }, utilisateur);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateUtilisateur(string id, [FromBody] Utilisateur utilisateur)
        {
            if (id != utilisateur.Id)
                return BadRequest();

            var updated = await _repository.UpdateUtilisateur(utilisateur);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteUtilisateur(string id)
        {
            var deleted = await _repository.DeleteUtilisateur(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}