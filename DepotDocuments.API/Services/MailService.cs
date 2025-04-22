using System.Net;
using System.Net.Mail;
using System.Text;
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

            // Corps HTML du mail
            string htmlBody = $"Madame, Monsieur {nomComplet},<br><br>" +
                "Nous accusons bonne réception de votre demande de crédit soumise via notre plateforme en ligne.<br><br>" +
                "Notre équipe étudiera attentivement votre dossier et vous tiendra informé(e) de la suite donnée à votre demande dans les plus brefs délais.<br><br>" +
                "Nous vous remercions pour votre confiance.<br><br>" +
                "Cordialement,<br>STB Bank";

            // Corps texte brut (fallback pour les lecteurs qui ne lisent pas le HTML)
            string plainBody = $"Madame, Monsieur {nomComplet},\n\n" +
                "Nous accusons bonne réception de votre demande de crédit soumise via notre plateforme en ligne.\n\n" +
                "Notre équipe étudiera attentivement votre dossier et vous tiendra informé(e) de la suite donnée à votre demande dans les plus brefs délais.\n\n" +
                "Nous vous remercions pour votre confiance.\n\n" +
                "Cordialement,\nSTB Bank";

            // Création du mail
            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, "STB Bank"),
                Subject = "Confirmation de la réception de votre demande de crédit",
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            // Ajout des deux versions du corps
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain"));
            message.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html"));

            // Destinataire
            message.To.Add(destinataire);

            // Configuration SMTP
            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            // Envoi
            await smtp.SendMailAsync(message);
        }
    }
}
