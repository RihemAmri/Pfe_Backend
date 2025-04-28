using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Validation.API.Services;
using Validation.API.Dtos;

namespace Validation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly IValidationService _service;

        public ValidationController(IValidationService service)
        {
            _service = service;
        }

        [HttpPost("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] ValidationRequestDto request)
        {
            var result = await _service.UpdateItemStatusAsync(id, request);
            if (!result)
                return NotFound("Élément non trouvé ou échec de mise à jour.");

            return Ok(new { message = "Statut mis à jour avec succès." });
        }
    }
}
