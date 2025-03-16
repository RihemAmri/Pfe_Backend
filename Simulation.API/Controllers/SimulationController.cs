using Microsoft.AspNetCore.Mvc;
using Simulation.API.Services;
using Simulation.API.Shared;

namespace Simulation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimulationController : ControllerBase
    {
        private readonly SimulationService _simulationService;

        public SimulationController(SimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        [HttpPost("simulate")]
        public ActionResult<SimulationResponse> Simulate([FromBody] SimulationRequest request)
        {
            var result = _simulationService.CalculateSimulation(request);
            return Ok(result);
        }
    }
}
