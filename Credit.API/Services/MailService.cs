using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Credit.API.Services
{
    public class MailService
    {
        private readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendMainleveeEmailAsync(string idClient, byte[] pdfBytes,string mail)
        {
            var smtpHost = _config["MailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
            var senderEmail = _config["MailSettings:SenderEmail"];
            var senderPassword = _config["MailSettings:SenderPassword"];

            var emailClient =mail;
            Console.WriteLine($"Email du client : {emailClient}");

            using var message = new MailMessage();
            message.From = new MailAddress(senderEmail, "STB Banque");
            message.To.Add(new MailAddress(emailClient));
            message.Subject = "Attestation de Mainlevée - STB Banque";
            message.Body = "Bonjour,\n\nVeuillez trouver en pièce jointe l’attestation de mainlevée de votre crédit.\n\nCordialement,\nSTB Banque";
            message.IsBodyHtml = false;

            // Ajouter la pièce jointe PDF
            using var ms = new System.IO.MemoryStream(pdfBytes);
            var attachment = new Attachment(ms, "Mainlevee.pdf", "application/pdf");
            message.Attachments.Add(attachment);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true // à adapter selon ton serveur SMTP
            };

            // SmtpClient ne supporte pas directement async, donc on fait un Task.Run pour l'envoi
            await Task.Run(() => smtpClient.Send(message));
        }

        private async Task<string> GetEmailClient(string idClient)
        {
            // Récupérer l'email en fonction de l'idClient (API ou DB)
            await Task.CompletedTask;
            return "client@example.com";
        }
    }
}
