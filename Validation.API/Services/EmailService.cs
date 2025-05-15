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

        public async Task SendValidationEmailAsync(string toEmail, byte[] pdfContent)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"]),
                Subject = "Félicitations - Accord de principe",
                Body = "Félicitations, votre demande a été validée. Vous trouverez ci-joint la notification d'accord de principe.",
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

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromAddress"]),
                Subject = "Votre code OTP",
                Body = $"Bonjour,\n\nVoici votre code OTP : {otp}\nCe code est valide pendant 5 minutes.\n\nCordialement,\nService Validation",
                IsBodyHtml = false // Mettre true si tu veux un format HTML
            };

            mailMessage.To.Add(destinataire);

            Console.WriteLine($"Envoi OTP à {destinataire}...");
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("OTP envoyé.");
        }

    }
}
