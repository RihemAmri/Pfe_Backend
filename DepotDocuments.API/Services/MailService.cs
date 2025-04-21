
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DepotDocuments.API.Services
{
    public class MailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnvoyerMailConfirmation(string destinataire, string nomComplet)
        {
            var smtpHost = _config["MailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
            var senderEmail = _config["MailSettings:SenderEmail"];
            var senderPassword = _config["MailSettings:SenderPassword"];

           var message = new MailMessage
    {
            From = new MailAddress(senderEmail, "STB Bank"),
            Subject = "Confirmation de la réception de votre demande de crédit",
            Body = $"Madame, Monsieur {nomComplet},\n\n" +
               "Nous accusons bonne réception de votre demande de crédit soumise via notre plateforme en ligne.\n\n" +
               "Notre équipe étudiera attentivement votre dossier et vous tiendra informé(e) de la suite donnée à votre demande dans les plus brefs délais.\n\n" +
               "Nous vous remercions pour votre confiance.\n\n" +
               "Cordialement,\nSTB Bank",
            IsBodyHtml = true
    };

            message.To.Add(destinataire);

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
