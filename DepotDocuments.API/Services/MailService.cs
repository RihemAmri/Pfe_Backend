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
                Subject = "🔔Confirmation de la réception de votre demande de crédit",
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
        public async Task EnvoyerNotificationAdmin()
{
    var smtpHost = _config["MailSettings:SmtpHost"];
    var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
    var senderEmail = _config["MailSettings:SenderEmail"];
    var senderPassword = _config["MailSettings:SenderPassword"];
    var adminEmail = _config["MailSettings:AdminEmail"];

    string htmlBody = $"<b>Nouvelle demande de crédit reçue</b><br><br>" +
                      "Un client vient de soumettre une demande de crédit via la plateforme.<br><br>" +
                      "Merci de consulter votre interface d’administration pour plus de détails.";

    string plainBody = $"Nouvelle demande de crédit reçue\n\n" +
                       "Un client vient de soumettre une demande de crédit via la plateforme.\n\n" +
                       "Merci de consulter votre interface d’administration pour plus de détails.";

    var message = new MailMessage
    {
        From = new MailAddress(senderEmail, "STB Bank - Notification"),
        Subject = "🔔 Nouvelle demande de crédit",
        SubjectEncoding = Encoding.UTF8,
        BodyEncoding = Encoding.UTF8
    };

    message.AlternateViews.Add(
        AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain"));
    message.AlternateViews.Add(
        AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html"));

    message.To.Add(adminEmail);

    using var smtp = new SmtpClient(smtpHost, smtpPort)
    {
        Credentials = new NetworkCredential(senderEmail, senderPassword),
        EnableSsl = true
    };

    await smtp.SendMailAsync(message);
}

        public async Task EnvoyerMailConfirmationAvecPdf(string destinataire, string nomComplet, byte[] pdfAttachment)
        {
            var smtpHost = _config["MailSettings:SmtpHost"];
            var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
            var senderEmail = _config["MailSettings:SenderEmail"];
            var senderPassword = _config["MailSettings:SenderPassword"];

            string htmlBody = $"Madame, Monsieur {nomComplet},<br><br>" +
                "Veuillez trouver en pièce jointe un récapitulatif signé de votre demande de crédit.<br><br>" +
                "Nous étudierons votre dossier dans les plus brefs délais.<br><br>" +
                "Cordialement,<br>STB Bank";

            string plainBody = $"Madame, Monsieur {nomComplet},\n\n" +
                "Veuillez trouver en pièce jointe un récapitulatif signé de votre demande de crédit.\n\n" +
                "Nous étudierons votre dossier dans les plus brefs délais.\n\n" +
                "Cordialement,\nSTB Bank";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, "STB Bank"),
                Subject = "📝 Votre demande de crédit signée",
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain"));
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html"));

            message.To.Add(destinataire);

            // Ajout du PDF en pièce jointe
            var attachment = new Attachment(new MemoryStream(pdfAttachment), "demande-signee.pdf", "application/pdf");
            message.Attachments.Add(attachment);

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
        public async Task EnvoyerOtpParMail(string destinataire, string code)
{
    var smtpHost = _config["MailSettings:SmtpHost"];
    var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
    var senderEmail = _config["MailSettings:SenderEmail"];
    var senderPassword = _config["MailSettings:SenderPassword"];

    string htmlBody = $"<p>Votre code de vérification est : <b>{code}</b></p><p>Il expire dans 5 minutes.</p>";
    string plainBody = $"Votre code de vérification est : {code}\nIl expire dans 5 minutes.";

    var message = new MailMessage
    {
        From = new MailAddress(senderEmail, "STB Bank"),
        Subject = "🔐 Code de vérification",
        SubjectEncoding = Encoding.UTF8,
        BodyEncoding = Encoding.UTF8
    };

    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainBody, Encoding.UTF8, "text/plain"));
    message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, Encoding.UTF8, "text/html"));
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
