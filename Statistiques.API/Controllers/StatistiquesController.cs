using Microsoft.AspNetCore.Mvc;
using Statistiques.API.Services;  
using Statistiques.API.DTO;       

namespace Declarations.API.Controllers
{
    [ApiController]
[Route("api/[controller]")]
public class StatistiquesController : ControllerBase
{
    private readonly IStatistiquesService _service;

    public StatistiquesController(IStatistiquesService service)
    {
        _service = service;
    }

    [HttpGet("totaux")]
    public async Task<IActionResult> GetTotaux()
    {
        var result = await _service.GetTotauxAsync();
        return Ok(result);
    }
    [HttpGet("demandes-par-mois")]
    public async Task<IActionResult> GetDemandesParMois()
    {
        var result = await _service.GetDemandesParMoisAsync();
        return Ok(result);
    }

    [HttpGet("credits-par-mois")]
    public async Task<IActionResult> GetCreditsParMois()
    {
        var result = await _service.GetCreditsParMoisAsync();
        return Ok(result);
    }
}

}