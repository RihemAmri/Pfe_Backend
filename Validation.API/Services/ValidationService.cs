using Microsoft.AspNetCore.Hosting; // Ajoute ça en haut
using System.Threading.Tasks;
using Validation.API.Dtos;
using Validation.API.Repositories;
using Validation.API.Services;
using Microsoft.Extensions.Configuration;

namespace Validation.API.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IValidationRepository _repo;
        private readonly PdfService _pdfService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public ValidationService(IValidationRepository repo, PdfService pdfService, EmailService emailService, IWebHostEnvironment env)
        {
            _repo = repo;
            _pdfService = pdfService;
            _emailService = emailService;
            _env = env;
        }

       public async Task<bool> UpdateItemStatusAsync(string id, ValidationRequestDto dto)
{
    if (string.IsNullOrEmpty(id) || dto == null || string.IsNullOrEmpty(dto.NewStatus))
        return false;

    string simplifiedStatus = SimplifyStatus(dto.NewStatus);
    var isUpdated = await _repo.UpdateDemandeStatusAsync(id, simplifiedStatus);

    if (isUpdated && simplifiedStatus == "validée")
    {
        var demande = await _repo.GetDemandeByIdAsync(id);

        var notificationData = new NotificationData
        {
            LogoPath = Path.Combine(_env.WebRootPath, "images", "Logo_STB.png"),
            Signature1Path = Path.Combine(_env.WebRootPath, "images", "signature1.png"),
            Signature2Path = Path.Combine(_env.WebRootPath, "images", "signature2.png"), // fallback
            NomPrenom = $"{demande.Nom} {demande.Prenom}",
            NumeroCompte = demande.NumeroCompte,
            MontantAccorde = $"{demande.MontantDemande} TND",
            Duree = $"{demande.DureeEnAnnees * 12} mois",
            TypeCredit = demande.TypeCredit,
            Conditions = "Assurance Vie",
            ConditionParticuliere = "Dépassement en compte interdit",
            DateNotification = DateTime.Now
        };

        // ✅ Convertir base64 vers byte[] si fourni depuis le front
        byte[] signatureAdminBytes = null;
        if (!string.IsNullOrEmpty(dto.SignatureAdminBase64))
        {
            try
            {
                signatureAdminBytes = Convert.FromBase64String(dto.SignatureAdminBase64);
            }
            catch (FormatException)
            {
                // Log ou gestion d'erreur
                signatureAdminBytes = null;
            }
        }

        // ✅ Générer le PDF
        var pdfContent = _pdfService.GenerateNotificationPdf(notificationData, signatureAdminBytes);
        Console.WriteLine($"Email cible : {demande.Email}");
        // ✅ Envoyer l'e-mail
        await _emailService.SendValidationEmailAsync(demande.Email, pdfContent);
    }
    else if (isUpdated && simplifiedStatus == "refusée")
        {
            var demande = await _repo.GetDemandeByIdAsync(id);
            if (demande != null)
            {
                var motif = string.IsNullOrWhiteSpace(dto.MotifRefus) ? "Aucun motif précisé." : dto.MotifRefus;

                await _emailService.SendRefusEmailAsync(demande.Email,$"{demande.Prenom} {demande.Nom}" , motif);
            }
        }
        else if (isUpdated && simplifiedStatus == "contrat_signé")
{
    var demande = await _repo.GetDemandeByIdAsync(id);
    if (demande != null)
    {
        await _emailService.SendContratEmailAsync(demande.Email,$"{demande.Prenom} {demande.Nom}", dto.MotifRefus);
    }
}
else if (isUpdated && simplifiedStatus == "crédit_actif")
{
    var demande = await _repo.GetDemandeByIdAsync(id);
    if (demande != null)
    {
        await _emailService.SendCreditActifEmailAsync(demande.Email,$"{demande.Prenom} {demande.Nom}", dto.MotifRefus);
    }
}
    return isUpdated;
}


        private string SimplifyStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "soumise";

            status = status.ToLower();
            if (status.Contains("refus"))
                return "refusée";
            if (status.Contains("valide") || status.Contains("accepté"))
                return "validée";
            if (status.Contains("contrat"))
                return "contrat_signé";
            if (status.Contains("actif"))
                return "crédit_actif";

            return "soumise";
        }
    }
}
