using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Validation.API.Dtos;
using Validation.API.DTO;
using Validation.API.Repositories;
using Microsoft.Extensions.Configuration;

namespace Validation.API.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IValidationRepository _repo;
        private readonly PdfService _pdfService;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;

        public ValidationService(
            IValidationRepository repo,
            PdfService pdfService,
            EmailService emailService,
            IWebHostEnvironment env,
            IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _pdfService = pdfService;
            _emailService = emailService;
            _env = env;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> UpdateItemStatusAsync(string id, ValidationRequestDto dto)
        {
            if (string.IsNullOrEmpty(id) || dto == null || string.IsNullOrEmpty(dto.NewStatus))
                return false;

            string simplifiedStatus = SimplifyStatus(dto.NewStatus);

            // Correction ici : on utilise dto.userId
            var isUpdated = await _repo.UpdateDemandeStatusAsync(id, simplifiedStatus);

            if (isUpdated)
            {
                var demande = await _repo.GetDemandeByIdAsync(id);
                if (demande == null) return false;

                switch (simplifiedStatus)
                {
                    case "validée":
                        var notificationData = new NotificationData
                        {
                            LogoPath = Path.Combine(_env.WebRootPath, "images", "Logo_STB.png"),
                            Signature1Path = Path.Combine(_env.WebRootPath, "images", "signature1.png"),
                            Signature2Path = Path.Combine(_env.WebRootPath, "images", "signature2.png"),
                            NomPrenom = $"{demande.Nom} {demande.Prenom}",
                            NumeroCompte = demande.NumeroCompte,
                            MontantAccorde = $"{demande.MontantDemande} TND",
                            Duree = $"{demande.DureeEnAnnees * 12} mois",
                            TypeCredit = demande.TypeCredit,
                            Conditions = "Assurance Vie",
                            ConditionParticuliere = "Dépassement en compte interdit",
                            DateNotification = DateTime.Now
                        };

                        byte[] signatureAdminBytes = null;
                        if (!string.IsNullOrEmpty(dto.SignatureAdminBase64))
                        {
                            try
                            {
                                signatureAdminBytes = Convert.FromBase64String(dto.SignatureAdminBase64);
                            }
                            catch (FormatException) { }
                        }

                        var pdfContent = _pdfService.GenerateNotificationPdf(notificationData, signatureAdminBytes);
                        await _emailService.SendValidationEmailAsync(demande.Email, pdfContent, dto.MotifRefus);
                        await EnvoyerNotificationAsync(demande.UserId, "validée",demande.TypeCredit, demande.MontantDemande);
                        break;

                    case "refusée":
                        var motif = string.IsNullOrWhiteSpace(dto.MotifRefus) ? "Aucun motif précisé." : dto.MotifRefus;
                        
                        await _emailService.SendRefusEmailAsync(demande.Email, $"{demande.Prenom} {demande.Nom}", motif);
                        
                        await EnvoyerNotificationAsync(demande.UserId, "refusée",demande.TypeCredit, demande.MontantDemande);
                        
                        break;

                    case "contrat_signé":
                        await _emailService.SendContratEmailAsync(demande.Email, $"{demande.Prenom} {demande.Nom}", dto.MotifRefus);
                        await EnvoyerNotificationAsync(demande.UserId, "contrat signé",demande.TypeCredit, demande.MontantDemande);
                        break;

                    case "crédit_actif":
                        await _emailService.SendCreditActifEmailAsync(demande.Email, $"{demande.Prenom} {demande.Nom}", dto.MotifRefus);
                        await EnvoyerNotificationAsync(demande.UserId, "crédit actif",demande.TypeCredit, demande.MontantDemande);
                        break;
                }
            }

            return isUpdated;
        }

        private string SimplifyStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "soumise";

            status = status.ToLower();
            if (status.Contains("refus")) return "refusée";
            if (status.Contains("valide") || status.Contains("accepté")) return "validée";
            if (status.Contains("contrat")) return "contrat_signé";
            if (status.Contains("actif")) return "crédit_actif";

            return "soumise";
        }

        private async Task EnvoyerNotificationAsync(string userId, string statut,string typeCredit, decimal montantCredit)
{
    var client = _httpClientFactory.CreateClient("NotificationApi");
     string montantStr = $"{montantCredit} TND";
    string creditInfo = $"crédit {typeCredit.ToUpper()}";

    string message = statut switch
    {
        "validée" => $"Votre {creditInfo} de {montantStr} a été validé avec succès.",
        "refusée" => $"Votre {creditInfo} de {montantStr} a été refusé. Veuillez consulter votre espace pour plus de détails.",
        "contrat signé" => $"Votre contrat pour le {creditInfo}de {montantStr}  est désormais signé.",
        "crédit actif" => $"Votre {creditInfo} de {montantStr} est désormais actif. Félicitations !",
        _ => $"Mise à jour de votre {creditInfo} : statut {statut}."
    };

    string type = statut switch
    {
        "validée" => "validation",
        "refusée" => "refus",
        "contrat signé" => "contrat",
        "crédit actif" => "crédit",
        _ => "update"
    };

    var notification = new CreateNotificationDto
    {
        DestinataireId = userId,
        Message = message,
        Date = DateTime.UtcNow,
        Lu = false,
        Type = "repdemande"
    };

    await client.PostAsJsonAsync("api/Notification", notification);
}

    }
}
