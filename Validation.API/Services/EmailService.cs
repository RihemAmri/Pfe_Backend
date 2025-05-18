using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Validation.API.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendValidationEmailAsync(string toEmail, byte[] pdfContent, string observation = null)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };
            var body = $@"
            <p>Félicitations,</p>
            <p>Votre demande de crédit a été <strong>validée</strong>. Vous trouverez ci-joint votre notification d'accord de principe.</p>
            <p style='color:#0056b3;'><strong>Important :</strong> Pour finaliser votre demande, veuillez vous rendre à votre agence afin de compléter les démarches (souscription à une assurance vie, signature du contrat, etc.).</p>"
            + (string.IsNullOrWhiteSpace(observation) ? "" : $@"<p><strong>Observation :</strong> {observation}</p>") + @"
            <p>Cordialement,</p>
            <p><em>L’équipe STB</em></p>";
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"], "STB Bank"),
                Subject = "Félicitations - Accord de principe",
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            mailMessage.Attachments.Add(new Attachment(new System.IO.MemoryStream(pdfContent), "Notification_Accord.pdf", "application/pdf"));
            Console.WriteLine($"Envoi de l'email à {toEmail}...");
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email envoyé.");
        }

        public async Task EnvoyerOtpParMail(string destinataire, string otp)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

           var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                <p>Cher administrateur,</p>
                <p>Voici votre code OTP :</p>
                <p style='font-size: 24px; color: green; font-weight: bold; margin-left: 20px;'>{otp}</p>
                <p>Ce code est valide pendant 5 minutes.</p>
                <p>Veuillez utiliser ce code pour finaliser l’opération en cours. Ne partagez ce code avec personne.</p>
                <p>Cordialement,<br/>Service Validation</p>
            </body>
            </html>";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"], "STB Bank"),
                Subject = "Votre code OTP",
                Body = body,
                IsBodyHtml = true // HTML activé pour le formatage
            };

            mailMessage.To.Add(destinataire);

            Console.WriteLine($"Envoi OTP à {destinataire}...");
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("OTP envoyé.");
        }


        public async Task SendRefusEmailAsync(string toEmail, string nomClient, string motif)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var subject = "Refus de votre demande de crédit";
            var body = $@"
Bonjour {nomClient},

Nous regrettons de vous informer que votre demande de crédit a été refusée.

Motif : {motif}

Nous comprenons que cette décision peut être décevante. Pour toute précision ou pour envisager d'autres solutions, n'hésitez pas à contacter votre agence STB.

Nous vous remercions de votre intérêt et restons à votre disposition.

Cordialement,  
L’équipe STB";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"], "STB Bank"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            Console.WriteLine($"Envoi de l'e-mail de refus à {toEmail}...");
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("E-mail de refus envoyé.");
        }
        public async Task SendContratEmailAsync(string toEmail, string nomClient, string observation = null)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var subject = "Contrat signé avec STB";
            var body = $@"
Bonjour {nomClient},

Nous vous informons que votre contrat a été signé avec succès.

{(string.IsNullOrWhiteSpace(observation) ? "" : $"Observation : {observation}\n")}

Merci pour votre confiance.

Cordialement,
L'équipe STB";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"], "STB Bank"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendCreditActifEmailAsync(string toEmail, string nomClient, string observation = null)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var subject = "Activation de votre crédit";
            var body = $@"
Bonjour {nomClient},

Votre crédit est désormais actif.

{(string.IsNullOrWhiteSpace(observation) ? "" : $"Observation : {observation}\n")}

N'hésitez pas à nous contacter pour toute question.

Cordialement,
L'équipe STB";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"], "STB Bank"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }


    }
}
