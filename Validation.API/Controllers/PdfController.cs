using Microsoft.AspNetCore.Mvc;
using Validation.API.Services;

namespace Validation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfController : ControllerBase
    {
        private readonly PdfService _pdfService;
        private readonly IWebHostEnvironment _env; // <- On ajoute ça pour récupérer le chemin wwwroot

        public PdfController(PdfService pdfService, IWebHostEnvironment env)
        {
            _pdfService = pdfService;
            _env = env;
        }

        [HttpGet("generate")]
        public IActionResult Generate()
        {
            // Récupérer le chemin absolu vers les images
            string logoPath = Path.Combine(_env.WebRootPath, "images", "Logo_STB.png");
            string signature1Path = Path.Combine(_env.WebRootPath, "images", "signature1.png");
            string signature2Path = Path.Combine(_env.WebRootPath, "images", "signature2.png");
            Console.WriteLine($"WebRootPath: {_env.WebRootPath}");
            var data = new NotificationData
            {
                LogoPath = logoPath,
                Signature1Path = signature1Path,
                Signature2Path = signature2Path,
                //Reference = "2025/1234",
                NomPrenom = "Nada Ben Salah",
                NumeroCompte = "123456789",
                MontantAccorde = "14 000,000 TND",
                Duree = "84 mois (07 ans)",
                TypeCredit = "Crédit FAS Investissement",
                Conditions = "Assurance Vie",
                ConditionParticuliere = "Dépassement en compte interdit",
                DateNotification = DateTime.Now
            };

            var pdfBytes = _pdfService.GenerateNotificationPdf(data);

            return File(pdfBytes, "application/pdf", "Notification_Accord.pdf");
        }
    }
}
