using Microsoft.AspNetCore.Mvc;

namespace Simulation.API.Repository
{
    public class SimulationRepository : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
