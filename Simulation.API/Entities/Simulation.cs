using Microsoft.AspNetCore.Mvc;

namespace Simulation.API.Entities
{
    public class Simulation : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
