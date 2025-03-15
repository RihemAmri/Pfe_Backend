using Microsoft.AspNetCore.Mvc;

namespace Simulation.API.Data
{
    public class ApplicationDbContext : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
